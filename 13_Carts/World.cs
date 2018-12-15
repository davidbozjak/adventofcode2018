using System;
using System.Collections.Generic;
using System.Linq;

namespace _13_Carts
{
    class World
    {
        public World(IEnumerable<Track> tracks, IEnumerable<Cart> carts)
        {
            this.Tracks = tracks.ToList();
            this.Carts = carts.ToList();
        }

        public IReadOnlyList<Track> Tracks { get; }

        public IList<Cart> Carts { get; }

        public void MakeStep()
        {
            IEnumerable<Cart> CartsInReadingOrder() => this.Carts.OrderBy(w => w.Position.Y * 1000 + w.Position.X);
            var cartsToMove = CartsInReadingOrder().ToList();

            while (cartsToMove.Count > 0)
            {
                var cart = cartsToMove[0];
                cartsToMove.Remove(cart);

                cart.Move();

                foreach (var collidingCart in CartsInReadingOrder())
                {
                    if (collidingCart == cart)
                    {
                        continue;
                    }

                    if (collidingCart.Position.Y > cart.Position.Y)
                    {
                        break;
                    }

                    if (cart.Position.X == collidingCart.Position.X &&
                        cart.Position.Y == collidingCart.Position.Y)
                    {
                        Console.WriteLine($"Collission at ({cart.Position.X}, {cart.Position.Y})");

                        this.Carts.Remove(cart);
                        this.Carts.Remove(collidingCart);
                        cartsToMove.Remove(collidingCart);
                    }
                }
            }
        }
    }
}
