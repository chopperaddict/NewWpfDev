using System;
using System . Collections;
using System . Collections . Generic;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace NewWpfDev . Views
{
    public class GenericStack<T> : IEnumerable<T>
    {
        private enum maxval : int{
            MAX = 10000
        } 

        private T [ ] values = new T [ 0];
        private int top = 0;

        public GenericStack(int max)
        {
            values = new T [ max];
        }
        public void Push ( T t )
        {
            
            values [ top ] = t;
            top++;
        }
        public T Pop ( )
        {
            top--;
            return values [ top ];
        }

        // This method implements the GetEnumerator method. It allows
        // an instance of the class to be used in a foreach statement.
        public IEnumerator<T> GetEnumerator ( )
        {
            for ( int index = top - 1 ; index >= 0 ; index-- )
            {
                yield return values [ index ];
            }
        }

        IEnumerator IEnumerable.GetEnumerator ( )
        {
            return GetEnumerator ( );
        }

        public IEnumerable<T> TopToBottom
        {
            get {
                for ( int index = top - 1  ; index >=0 ; index-- )
                {
                    yield return values [ index ];
                }
            }
        }

        public IEnumerable<T> BottomToTop
        {
            get
            {
                for ( int index = 0 ; index <= top - 1 ; index++ )
                {
                    yield return values [ index ];
                }
            }
        }

        public IEnumerable<T> TopN ( int itemsFromTop )
        {
            // Return less than itemsFromTop if necessary.
            int startIndex = itemsFromTop >= top ? 0 : top - itemsFromTop;

            for ( int index = top - 1 ; index >= startIndex ; index-- )
            {
                yield return values [ index ];
            }
        }

    }
}
