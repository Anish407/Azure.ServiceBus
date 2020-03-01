﻿using System;

namespace Session.COmmon
{
    public class RfidTag
    {
       
        public string TagId { get; set; }
       
        public string Product { get; set; }

        public double Price { get; set; }


        public RfidTag()
        {
            TagId = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return $"Product:{ Product }\tPrice:${ Price }";
        }
    }
}
