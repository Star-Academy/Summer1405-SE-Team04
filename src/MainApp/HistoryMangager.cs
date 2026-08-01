namespace G5
{

    class HistoryManager
    {
        private readonly Stack<string> _backHistory = new Stack<string>();
        private readonly Stack<string> _forwardHistory = new Stack<string>();
        private readonly Dictionary<string, int> _searchesCount = new Dictionary<string, int>();
        public void Search(string query)
        {
            _backHistory.Push(query);
            _forwardHistory.Clear();
            _searchesCount[query] = _searchesCount.GetValueOrDefault(query, 0) + 1;
        }
        public bool Back()
        {
            if (_backHistory.Count == 1)
                return false;

            _forwardHistory.Push(_backHistory.Pop());
            return true;
        }
        public bool Forward()
        {
            if (_forwardHistory.Count == 0)
                return false;

            _backHistory.Push(_forwardHistory.Pop());
            return true;
        }
        public List<KeyValuePair<string, int>> Stats(int count)
        {
            return _searchesCount.OrderBy(entry => -entry.Value).Take(count).ToList();
        }

        public int Unique()
        {
            return _searchesCount.Keys.Count;
        }

        public string CurrentPage()
        {
            if (_backHistory.Count == 0)
                return null;
            else
                return _backHistory.Peek();
        }
    }
}