/*
* Name: [YOUR NAME HERE]
* South Hills Username: [YOUR SOUTH HILLS USERNAME HERE]
*/
#pragma warning disable CS8618
#pragma warning disable CS8602
#pragma warning disable CS8604
using System;
using System.Collections;
using System.Collections.Generic;

namespace ComplexityLab
{
    public class Source
    {
        //Do not change the name or accessors for the following property:
        public ICollection<ulong> Collection { get; private set; }

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
