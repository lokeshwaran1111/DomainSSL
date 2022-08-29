﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domainssl.Models
{
    public class Domain
    {
       
        public string Domain_name { get; set; }
        public string Issued_by { get; set; }
        public string Issued_to { get; set; }
        public long Issued_on { get; set; }
        public long Expires_on { get; set; }
        public Boolean Auto_renewal_enabled { get; set; }
       // public string Isdeleted { get; set; }
    }

}
