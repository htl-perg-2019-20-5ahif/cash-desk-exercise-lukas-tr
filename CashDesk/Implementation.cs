using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {

        private CashDeskContext context;

        /// <inheritdoc />
        public async Task InitializeDatabaseAsync()
        {
            if (context != null)
            {
                throw new InvalidOperationException();
            }
            context = new CashDeskContext();
        }

        /// <inheritdoc />
        public async Task<int> AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            if (context.Members.Count(m => m.LastName == lastName) > 0)
            {
                throw new DuplicateNameException();
            }
            var member = new Member { Birthday = birthday, FirstName = firstName, LastName = lastName };
            await context.Members.AddAsync(member);
            await context.SaveChangesAsync();
            return member.MemberNumber;
        }

        /// <inheritdoc />
        public async Task DeleteMemberAsync(int memberNumber)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
        }
        /// <inheritdoc />
        public async Task<IMembership> JoinMemberAsync(int memberNumber)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var existingMembership = context.Memberships.FirstOrDefault(m => m.MemberId == memberNumber);
            if (existingMembership != null && existingMembership.IsActive)
            {
                throw new AlreadyMemberException();
            }
            var membership = existingMembership != null ? existingMembership : new Membership { MemberId = memberNumber, Begin = DateTime.Now, Deposits = new List<Deposit>() };
            membership.IsActive = true;
            if (existingMembership == null)
            {
                await context.Memberships.AddAsync(membership);
            }
            await context.SaveChangesAsync();
            return membership;
        }

        /// <inheritdoc />
        public async Task<IMembership> CancelMembershipAsync(int memberNumber)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var membership = context.Memberships.FirstOrDefault(m => m.MemberId == memberNumber);
            if (membership == null)
            {
                throw new NoMemberException();
            }
            membership.End = DateTime.Now;
            membership.IsActive = false;
            await context.SaveChangesAsync();
            return membership;
        }

        /// <inheritdoc />
        public async Task DepositAsync(int memberNumber, decimal amount)
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var membership = context.Memberships.FirstOrDefault(m => m.MemberId == memberNumber);
            if (membership == null)
            {
                throw new NoMemberException();
            }
            var deposit = new Deposit { Amount = amount, Membership = membership };
            membership.Deposits.Add(deposit);
            // await context.Deposits.AddAsync(deposit);
            await context.SaveChangesAsync();
            return;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IDepositStatistics>> GetDepositStatisticsAsync()
        {
            if (context == null)
            {
                throw new InvalidOperationException();
            }
            var members = context.Members.Where(m => m.Membership != null);
            var statistics = new List<DepositStatistics>();
            foreach(var member in members)
            {
                decimal amount = 0;
                int year = 0;
                if(member.Membership != null)
                {
                    year = member.Membership.Begin.Year;
                }
                if(member.Membership != null && member.Membership.Deposits != null && member.Membership.Deposits.Count > 0)
                {
                    foreach(var d in member.Membership.Deposits)
                    {
                        amount += d.Amount;
                    }
                }
                statistics.Add(new DepositStatistics { Member = member, TotalAmount = amount, Year = year });
            }
            return statistics;
        }

        /// <inheritdoc />
        public async void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
            }
        }
    }
}
