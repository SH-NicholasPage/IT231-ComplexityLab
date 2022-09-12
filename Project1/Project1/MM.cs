/*
* Name: [YOUR NAME HERE]
* South Hills Username: [YOUR SOUTH HILLS USERNAME HERE]
*/

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComplexityLab
{
    public class ArrayContainer
    {
        public ICollection ULArray { get; private set; }

        public bool Init()
        {
            ULArray = new List<ulong>();
            return true;
        }

        public bool Insert(ulong item)
        {
            ((List<ulong>)ULArray!).Add(item);
            ((List<ulong>)ULArray!).Sort();
            return true;
        }

        public bool Remove(ulong item)
        {
            ((List<ulong>)ULArray).Remove(item);
            return true;
        }
    }
}
