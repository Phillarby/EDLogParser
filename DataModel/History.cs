//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class History
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public History()
        {
            this.Journeys = new HashSet<Journey>();
            this.Journeys1 = new HashSet<Journey>();
        }
    
        public int Id { get; set; }
        public int ProfileID { get; set; }
        public System.DateTime Date { get; set; }
        public int SystemID { get; set; }
    
        public virtual Asset Asset { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual System System { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journey> Journeys { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journey> Journeys1 { get; set; }
    }
}
