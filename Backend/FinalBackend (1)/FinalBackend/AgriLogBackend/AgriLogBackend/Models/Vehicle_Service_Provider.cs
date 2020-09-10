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
    
    public partial class Vehicle_Service_Provider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehicle_Service_Provider()
        {
            this.Vehicle_Repair = new HashSet<Vehicle_Repair>();
            this.Vehicle_Service = new HashSet<Vehicle_Service>();
        }
    
        public int Provider_ID { get; set; }
        public string Provider_Name { get; set; }
        public string Provider_Contact_Number { get; set; }
        public string Provider_Address { get; set; }
        public string Is_Active { get; set; }
        public string Provider_Email { get; set; }
        public int Farm_ID { get; set; }
    
        public virtual Farm Farm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vehicle_Repair> Vehicle_Repair { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vehicle_Service> Vehicle_Service { get; set; }
    }
}
