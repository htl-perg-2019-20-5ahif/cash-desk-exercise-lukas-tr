using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CashDesk
{
    public class Member : IMember
    {
        [Key]
        public int MemberNumber { get; set; }

        public string FirstName { get; set; } 

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public Membership Membership { get; set; }
    }

    public class Membership : IMembership
    {
        public int MembershipId { get; set; }
        public Member Member { get; set; }
        public int MemberId { get; set; }

        public DateTime Begin  { get; set; }

        public DateTime End  { get; set; }

        public bool IsActive { get; set; }

        public List<Deposit> Deposits { get; set; }

        [NotMapped]
        IMember IMembership.Member => Member;
    }

    public class Deposit : IDeposit
    {
        public int DepositId { get; set; }

        public Membership Membership { get; set; }

        public decimal Amount { get; set; }

        [NotMapped]
        IMembership IDeposit.Membership => Membership;
    }

    public class DepositStatistics : IDepositStatistics
    {
        public Member Member { get; set; }

        public int Year  { get; set; }

        public decimal TotalAmount  { get; set; }

        IMember IDepositStatistics.Member => Member;
    }
}

