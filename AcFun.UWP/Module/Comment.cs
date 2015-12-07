using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Module
{
    public class Comment
    {
        private static Comment _instance;
        public static Comment Instance => _instance ?? (_instance = new Comment());

        private Comment()
        {
        }

        public void SetQuote(int quoteId,int quoteFloor)
        {
            QuoteId = quoteId;
            QuoteFloor = quoteFloor;
        }

        public EventHandler<int> QuoteIdChanged;
        public EventHandler<int> QuoteFloorChanged;

        private void OnQuoteIdChanged()
        {
            QuoteIdChanged?.Invoke(this, QuoteId);
        }

        private void OnQuoteFloorChanged()
        {
            QuoteFloorChanged?.Invoke(this, QuoteFloor);
        }

        private int _quoteId = -1;
        public int QuoteId
        {
            get { return _quoteId; }
            set
            {
                _quoteId = value;
                OnQuoteIdChanged();
            }
        }

        private int _quoteFloor = -1;
        public int QuoteFloor
        {
            get { return _quoteFloor; }
            set
            {
                _quoteFloor = value;
                OnQuoteFloorChanged();
            }
        }
    }
}
