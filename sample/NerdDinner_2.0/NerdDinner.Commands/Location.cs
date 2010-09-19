namespace NerdDinner.Commands
{
    public class Location
    {
        public string Address { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(Location)) return false;
            return Equals((Location)obj);
        }

        public bool Equals(Location other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return Equals(other.Address, Address) && Equals(other.Country, Country) && other.Latitude.Equals(Latitude) && other.Longitude.Equals(Longitude);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Address != null ? Address.GetHashCode() : 0);
                result = (result * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                result = (result * 397) ^ Latitude.GetHashCode();
                result = (result * 397) ^ Longitude.GetHashCode();
                return result;
            }
        }
    }
}