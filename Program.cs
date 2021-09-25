using System;
using System.Collections.Generic;
using System.IO;

namespace ListSerialization
{
    
    class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count = 0;

        public void Serialize(Stream s)
        {
            List<ListNode> nodeArray = new List<ListNode>();
            ListNode currentlyProcessedNode = Head;
            do
            {
                nodeArray.Add(currentlyProcessedNode);
            } while ((currentlyProcessedNode = currentlyProcessedNode.Next) != null);

            using (StreamWriter sw = new StreamWriter(s))
            {
                foreach (ListNode node in nodeArray)
                {
                    sw.WriteLine(node.Data.ToString() + ":" + nodeArray.IndexOf(node.Random).ToString());
                }
            }
        }

        public void Deserialize(Stream s)
        {
            List<ListNode> nodeArray = new List<ListNode>();
            ListNode currentlyProcessedNode = new ListNode();
            Head = currentlyProcessedNode;
            Count = 0;
            string line;

            using (StreamReader sr = new StreamReader(s))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Equals(""))
                        continue;
                    Count++;
                    currentlyProcessedNode.Data = line;
                    ListNode next = new ListNode();
                    currentlyProcessedNode.Next = next;
                    nodeArray.Add(currentlyProcessedNode);
                    next.Previous = currentlyProcessedNode;
                    currentlyProcessedNode = next;
                }
            }

            Tail = currentlyProcessedNode.Previous;
            Tail.Next = null;

            foreach (ListNode node in nodeArray)
            {
                string[] data = node.Data.Split(':');
                node.Data = data[0];
                node.Random = nodeArray[Convert.ToInt32(data[1])];
            }

        }

        public void AddRandomNode()
        {
            Count++;
            Random random = new Random();
            ListNode newNode = new ListNode();
            newNode.Previous = Tail;
            newNode.Data = random.Next(0,1000).ToString();
            if(Head == null)
                Head = newNode;
            else
                Tail.Next = newNode;
            Tail = newNode;
            
        }

        public void RandomizeNodes()
        {
            Random random = new Random();
            ListNode currentlyProcessedNode = Head;
            do
            {
                int nodeIndex = random.Next(0, Count);
                ListNode selectedNode = Head;
                for(int currNodeIndex = 0; currNodeIndex < nodeIndex; currNodeIndex++)
                {
                    selectedNode = selectedNode.Next;
                }
                currentlyProcessedNode.Random = selectedNode;
            } while((currentlyProcessedNode = currentlyProcessedNode.Next) != null);
        }

        public bool Equals (ListRand toCompare)
        {
            ListNode firstItem = Head;
            ListNode secondItem = toCompare.Head;
            do
            {
                if(!firstItem.Data.Equals(secondItem.Data) && !firstItem.Random.Data.Equals(secondItem.Random.Data))
                    return false;
            } while((firstItem = firstItem.Next) != null && (secondItem = secondItem.Next) != null);
            return true;
        }
     }

    

    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            int length = 7;

            ListRand first = new ListRand();

            for (int i = 0; i < length; i++)
                first.AddRandomNode();

            first.RandomizeNodes();

            FileStream fs = new FileStream("list.txt", FileMode.OpenOrCreate);
            first.Serialize(fs);
            fs.Close();

            ListRand second = new ListRand();
            fs = new FileStream("list.txt", FileMode.Open);
            second.Deserialize(fs);
            fs.Close();


            if (first.Equals(second)) 
                Console.WriteLine("Списки идентичны");
            Console.Read();
            
        }
    }
}