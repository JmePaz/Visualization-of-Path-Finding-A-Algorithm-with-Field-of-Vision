using System;
class PriorityQueue<T> where T: IComparable { 

        private T[] heapArr;

        private int capacity;
        private int currHeapKey;

        public PriorityQueue(int capacity = 50)
        {
            this.capacity = capacity;
            this.heapArr = new T[capacity];
            this.heapArr[0] = default(T);
            this.currHeapKey = 1;
        }

        protected internal void Enqueue(T item)
        {
            if (this.currHeapKey == capacity)
                Expand();

            this.heapArr[this.currHeapKey++] = item;
            HeapifyUp(this.currHeapKey);
        }

        protected internal T Dequeue()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Error Priority Queue is Empty");

            //alternate
            T itemDequeued = this.heapArr[1];
            this.heapArr[1] = this.heapArr[this.currHeapKey - 1];
            this.heapArr[this.currHeapKey-1] = default(T);

            // decrease by 1
            this.currHeapKey--;
            //heapify down        
            HeapifyDown(1);

            return itemDequeued;
        }
        protected internal T PeekHead()
        {
            if(this.IsEmpty())
                    throw new InvalidOperationException("Error Priority Queue is Empty");
            return this.heapArr[1];
        }

        private void Swap(int heapKey1, int heapKey2)
        {
            T temp = this.heapArr[heapKey1];
            this.heapArr[heapKey1] = this.heapArr[heapKey2];
            this.heapArr[heapKey2] = temp;
        }
        private void HeapifyUp(int heapTempKey)
        {
            if (heapTempKey-1 <= 1)
            {
                return;
            }

            // current heap key
            T currChild = this.heapArr[heapTempKey - 1];
            // parent
            T parent = this.heapArr[(heapTempKey - 1) / 2];

            if (LessThan(currChild,parent))
            {
                //swap two item
                Swap(heapTempKey - 1, (heapTempKey - 1) / 2);
            }
            //move to another node
            HeapifyUp(((heapTempKey - 1) / 2) + 1);

        }


        private void HeapifyDown(int heapTempKey)
        {
            //base case
            if (heapTempKey * 2 >= this.currHeapKey)
            {
                return;
            }

            // get all values
            T parent = GetItem(heapTempKey);
            T leftChild = GetItem(heapTempKey * 2);
            T rightChild = GetItem(heapTempKey * 2 + 1);

            if (!IsNull(leftChild)  && LessThan(leftChild, parent) && ( IsNull(rightChild) || LessThanEqual(leftChild, rightChild) ))
            {
                
                //go to left
                Swap(heapTempKey, heapTempKey * 2);
                HeapifyDown(heapTempKey * 2);
            }
            else if (!IsNull(rightChild) && LessThan(rightChild, parent) && LessThan(rightChild,leftChild))
            {
                //go to right
                Swap(heapTempKey, heapTempKey * 2 + 1);
                HeapifyDown(heapTempKey * 2 + 1);
            }
        }

        protected internal void DisplayArr()
        {
           for(int i=1; i<this.currHeapKey; i++)
            {
                T curr = this.heapArr[i];
                Console.WriteLine(curr);
            }
            Console.WriteLine(' ');
        }
        protected internal T GetItem(int nthIndex)
        {
            if (nthIndex >= this.currHeapKey)
            {
                return default(T);
            }
            return this.heapArr[nthIndex];
        }

        private void Expand()
        {
            this.capacity = this.capacity + (this.capacity / 2) + 1;
            T[] newHeapArr = new T[this.capacity];
            for(int i=0; i<heapArr.Length; i++)
            {
                newHeapArr[i] = heapArr[i];
            }
            heapArr = newHeapArr;

        }

        protected internal bool IsEmpty()
        {
            return this.currHeapKey == 1;
        }


        // operators
        private bool LessThan(T a, T b)
        {
            return a.CompareTo(b) < 0;
        }
    
        private bool LessThanEqual(T a, T b)
        {
            return a.CompareTo(b) <= 0;
        }
        private bool Equal(T a, T b)
        {

            return a.CompareTo(b) == 0;
        }

        private bool IsNull(T a)
        {
            return Equals(a, default(T));
        }

    
}