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
    
    public partial class Fault_Type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Fault_Type()
        {
            this.Fault_Log = new HashSet<Fault_Log>();
        }
    
        public int FT_ID { get; set; }
        public string FT_Description { get; set; }
        public Nullable<int> Farm_ID { get; set; }
    
        public virtual Farm Farm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fault_Log> Fault_Log { get; set; }
    }
}
