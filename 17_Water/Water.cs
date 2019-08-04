namespace _17_Water
{
    interface IWater
    {
        int Id { get; }

        bool IsStuck { get; set; }
    }

    class WaterFactory
    {
        private int count = 0;

        public IWater GetWater()
        {
            return new Water(count++);
        }

        private sealed class Water : IWater
        {
            public int Id { get; }

            private bool isStuck = false;
            public bool IsStuck
            {
                get => this.isStuck;
                set
                {
                    this.isStuck = this.isStuck || value;
                }
            }

            public Water(int id)
            {
                this.Id = id;
            }
        }
    }
}
