using OnlineLibrary.Common.DBEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OnlineLibrary.Common.EntityProcessing.Sorting
{
    public class SortingOptions
    {
        private string propertyToOrder;
        public string PropertyToOrder
        {
            get { return propertyToOrder; }
            set
            {
                propertyToOrder =
                        String.IsNullOrWhiteSpace(value) ?
                            "Id" :
                            typeof(Book).GetProperty(value.Trim()) == null ?
                            "Id" : value.Trim();
            }
        }

        private ListSortDirection sortDirection;

        public ListSortDirection SortDirection
        {
            get { return sortDirection; }
            set
            {
                sortDirection =
                        !Enum.IsDefined(typeof(ListSortDirection), value) ? 
                            ListSortDirection.Ascending : value;
            }
        }

        public SortingOptions()
        {
            PropertyToOrder = "Id";
            SortDirection = ListSortDirection.Ascending;
        }
    }
}
