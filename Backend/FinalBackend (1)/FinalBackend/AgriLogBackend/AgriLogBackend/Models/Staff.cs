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
    
    public partial class Staff
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staff()
        {
            this.Clockeds = new HashSet<Clocked>();
            this.Fault_Staff = new HashSet<Fault_Staff>();
            this.Maintenance_Staff = new HashSet<Maintenance_Staff>();
            this.Staff_Notification_Log = new HashSet<Staff_Notification_Log>();
            this.Staff_Skill = new HashSet<Staff_Skill>();
            this.Task_Task_Schedule = new HashSet<Task_Task_Schedule>();
        }
    
        public string Staff_ID { get; set; }
        public int Farm_ID { get; set; }
        public string Staff_Name { get; set; }
        public string Staff_Surname { get; set; }
        public string Staff_Phone_Number { get; set; }
        public System.DateTime Staff_DoB { get; set; }
        public string Staff_Photo { get; set; }
        public string Staff_Address { get; set; }
        public string Is_Active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Clocked> Clockeds { get; set; }
        public virtual Farm Farm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fault_Staff> Fault_Staff { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance_Staff> Maintenance_Staff { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Staff_Notification_Log> Staff_Notification_Log { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Staff_Skill> Staff_Skill { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Task_Task_Schedule> Task_Task_Schedule { get; set; }
    }
}
