using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace PortfoyYonetimSistemi.Entities
{
    [Table("Customers")]
    public class Customer : FullAuditedEntity<long>
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerCode { get; set; }

        public CustomerCustomerType CustomerType { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public CustomerRiskProfile? RiskProfile { get; set; }

        public decimal? TotalAum { get; set; }

        public DateTime? OnboardingDate { get; set; }

        public long PortfolioManagerId { get; set; }

        [ForeignKey(nameof(PortfolioManagerId))]
        public virtual PortfolioManager PortfolioManager { get; set; }

        public virtual ICollection<CustomerManagerTransfer> CustomerManagerTransfers { get; set; }

        public virtual ICollection<CustomerFundHolding> CustomerFundHoldings { get; set; }

    }
}