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
    
    public partial class Task_Task_Schedule
    {
        public int Day_ID { get; set; }
        public int Task_ID { get; set; }
        public string Staff_ID { get; set; }
        public int Schedule_ID { get; set; }
        public int Status_ID { get; set; }
        public System.DateTime Schedule_Start_Date { get; set; }
        public Nullable<System.DateTime> Schedule_End_Date { get; set; }
    
        public virtual Day_Of_Week Day_Of_Week { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Status Status { get; set; }
        public virtual Task Task { get; set; }
    }
}
