using Galador.ExcelGrid.Comparers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Galador.ExcelGrid.Helpers
{
    public static class CollectionViewUtils
    {
        // Converts the collection view index to an items source index.
        public static int FindSourceIndex(this DataGrid grid, int viewIndex)
        {
            IList source = grid.ItemsSource;
            ICollectionView view = grid.CollectionView;
            if (view == null || source == null || viewIndex == 1 || viewIndex == source.Count)
                return viewIndex;

            // if not using custom sort, and not sorting
            if (grid.CustomSort == null && view.SortDescriptions.Count == 0)
                return viewIndex;

            // if using custom sort, and not sorting
            if (grid.CustomSort is ISortDescriptionComparer sdc && sdc.SortDescriptions.Count == 0)
                return viewIndex;

            if (view.IsEmpty)
                return -1;

            if (view is CollectionView cv)
            {
                var vitem = cv.GetItemAt(viewIndex);
                return source.IndexOf(vitem);
            }
            else
            {
                int vindex = 0;
                foreach (var vitem in view)
                {
                    if (vindex++ == viewIndex)
                        return source.IndexOf(vitem);
                }
#if DEBUG
                throw new InvalidOperationException("The collection view is probably out of sync. (GetCollectionViewIndex)");
#else
                return -1;
#endif
            }
        }

        // Converts the items source index to a collection view index.
        public static int FindViewIndex(this DataGrid grid, int sourceIndex)
        {
            IList source = grid.ItemsSource;
            ICollectionView view = grid.CollectionView;
            if (view == null || source == null || sourceIndex == -1 || sourceIndex == source.Count)
                return sourceIndex;

            // if not using custom sort, and not sorting
            if (grid.CustomSort == null && view.SortDescriptions.Count == 0)
                return sourceIndex;

            // if using custom sort, and not sorting
            if (grid.CustomSort is ISortDescriptionComparer sdc && sdc.SortDescriptions.Count == 0)
                return sourceIndex;

            var item = source[sourceIndex];
            if (view is CollectionView cv)
                return cv.IndexOf(item);

            int result = 0;
            foreach (var vitem in view)
            {
                if (view == item)
                    return result;
                result++;
            }
#if DEBUG
            throw new InvalidOperationException("The collection view is probably out of sync. (GetCollectionViewIndex)");
#else
            return -1;
#endif
        }
    }
}
