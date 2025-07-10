using System;
using System.Collections.Generic;
using TGramHunt.Contract.Enums;
using TGramHunt.Contract.ViewModels.MainPage;

namespace TGramHunt.Contract.Helpers
{
    public class ProductComparer : IComparer<ProductBase>
    {
        private readonly ProductListSorting? _sorting;

        public ProductComparer(ProductListSorting? sorting)
        {
            this._sorting = sorting;
        }

        public int Compare(ProductBase? x, ProductBase? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null)
            {
                return 1;
            }
            else if (y == null)
            {
                return -1;
            }

            if (!this._sorting.HasValue || this._sorting == ProductListSorting.mostPopular)
            {
                return this.SortByVote(x, y, 0);
            }
            else if (this._sorting == ProductListSorting.newest)
            {
                return this.SortByDateOfCreation(x, y, 0);
            }

            return 0;
        }

        private int SortByDateOfCreation(
            ProductBase? x,
            ProductBase? y,
            int lvl)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (x.DateOfCreation == y.DateOfCreation)
            {
                if (lvl >= 1)
                {
                    return 0;
                }
                else
                {
                    ++lvl;
                    return this.SortByVote(x, y, lvl);
                }
            }
            else if (x.DateOfCreation > y.DateOfCreation)
            {
                return -1;
            }
            else if (x.DateOfCreation < y.DateOfCreation)
            {
                return 1;
            }

            return 0;
        }

        private int SortByVote(
            ProductBase? x,
            ProductBase? y,
            int lvl)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            if (x.Votes == y.Votes)
            {
                if (lvl >= 1)
                {
                    return 0;
                }
                else
                {
                    ++lvl;
                    return this.SortByDateOfCreation(x, y, lvl);
                }
            }
            else if (x.Votes < y.Votes)
            {
                return 1;
            }
            else if (x.Votes > y.Votes)
            {
                return -1;
            }

            return 0;
        }
    }
}
