/*
* Name: [YOUR NAME HERE]
* South Hills Username: [YOUR SOUTH HILLS USERNAME HERE]
*/
#pragma warning disable CS8618
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
    public class Source
    {
        public ICollection Collection { get; private set; }

        public bool Init()
        {
            //TODO: Initialize your collection data structure and anything else you want to do before recieving inputs.
            //Return true if you were able to initialize properly. If there was an error, return false.
            return true;
        }

        public bool Insert(ulong item)
        {
            //TODO: Insert the item into the collection.
            //Return true if you were able to put the item into the collection. If there was an error, return false.
            return true;
        }

        public bool Remove(ulong item)
        {
            //TODO: Remove the item from your collection.
            //Return true if you were able to remove the item from the collection. If there was an error, return false.
            return true;
        }
    }
}
