using System;

namespace NerdDinner.Commands
{
    public class DinnerHost
    {
        public Guid HostedById { get; set; }
        public string HostedBy { get; set; }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(DinnerHost)) return false;
            return Equals((DinnerHost)obj);
        }

        public bool Equals(DinnerHost other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return other.HostedById.Equals(HostedById) && Equals(other.HostedBy, HostedBy);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (HostedById.GetHashCode() * 397) ^ (HostedBy != null ? HostedBy.GetHashCode() : 0);
            }
        }
    }
}