//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AgriLogBackend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Audit_Trail
    {
        public long Audit_Log_ID { get; set; }
        public int User_ID { get; set; }
        public Nullable<int> Farm_ID { get; set; }
        public string User_Action { get; set; }
        public System.DateTime Action_DateTime { get; set; }
        public long Affected_ID { get; set; }
    
        public virtual Farm Farm { get; set; }
        public virtual User User { get; set; }
    }
}
