namespace _13_Carts
{
    class ConnectedTracks
    {
        private readonly Track[] tracs = new Track[4];

        public Track this[Direction index]
        {
            get => this.tracs[(int)index];
            set => this.tracs[(int)index] = value;
        }
    }
}
