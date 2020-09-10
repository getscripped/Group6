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
    
    public partial class Equipment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Equipment()
        {
            this.Fault_Log = new HashSet<Fault_Log>();
            this.Maintenance_Log = new HashSet<Maintenance_Log>();
            this.Tasks = new HashSet<Task>();
        }
    
        public int Equipment_ID { get; set; }
        public Nullable<int> Equipment_Type_ID { get; set; }
        public Nullable<int> Infrastructure_ID { get; set; }
        public string Equipment_Description { get; set; }
        public string Equipment_Condition { get; set; }
        public string Equipment_Cost { get; set; }
        public string Is_Active { get; set; }
        public int Farm_ID { get; set; }
        public Nullable<int> Section_ID { get; set; }
    
        public virtual Equipment_Type Equipment_Type { get; set; }
        public virtual Farm Farm { get; set; }
        public virtual Infrastructure Infrastructure { get; set; }
        public virtual Section Section { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fault_Log> Fault_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance_Log> Maintenance_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
