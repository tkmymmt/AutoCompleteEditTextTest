using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Widget;
using Android.OS;
using Android;
using Android.Views;
using Android.Content;
using Android.Runtime;
using Android.Util;


namespace AutoCompleteEditTextTest
{
    public class AutoCompleteEditText : Android.Support.Constraints.ConstraintLayout
    {
        public int MaxSuggestCount { get; set; } = 5;

        EditText _inputField;
        ListView _autoCompleteArea;
        ArrayAdapter _adapter;

        List<string> _autoCompleteWords;
        string _previousWord;
        List<string> _previousList;
        string[] _currentList = new string[0];

        public AutoCompleteEditText(Context context) :
            base(context)
        {
            Initialize();
        }

        public AutoCompleteEditText(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public AutoCompleteEditText(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        void Initialize()
        {
            _autoCompleteWords = new List<string>
            {
                "apple",
                "pineapple",
                "mikan",
                "ringo",
                "pear",
                "strawberry",
                "blueberry",
                "bananas"
            };

            var view = LayoutInflater.FromContext(Context).Inflate(Resource.Layout.auto_complete_edit_text, this);
            _inputField = view.FindViewById<EditText>(Resource.Id.inputField);
            _inputField.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => 
            {
                SearchWord(e.Text.ToString());
                _autoCompleteArea.Visibility = ViewStates.Visible;
            };

            _autoCompleteArea = view.FindViewById<ListView>(Resource.Id.autocomplete_area);
            _adapter = new ArrayAdapter(Context, Resource.Layout.listview_item, _currentList);
            _autoCompleteArea.Adapter = _adapter;
            _autoCompleteArea.ItemClick += (sender, e) =>
            {
                _inputField.Text = _autoCompleteArea.GetItemAtPosition(e.Position).ToString();
                _currentList = new string[0];
                _autoCompleteArea.Visibility = ViewStates.Gone;
            };
        }

        void SearchWord(string word)
        {
            if(string.IsNullOrEmpty(word))
            {
                _currentList = new string[0];
                _adapter = new ArrayAdapter(Context, Resource.Layout.listview_item, _currentList);
                _autoCompleteArea.Adapter = _adapter;
                return;
            }

            _currentList = _autoCompleteWords.Where(s => IsSimilerWord(word, s)).Take(MaxSuggestCount).ToArray();
            _adapter = new ArrayAdapter(Context, Resource.Layout.listview_item, _currentList);
            _autoCompleteArea.Adapter = _adapter;
        }

        bool IsSimilerWord(string inputWord, string targetWord)
        {
            if (targetWord.Contains(inputWord)) return true;

            int searchTargetIndex = 0;
            for(int i = 0; i < inputWord.Length; i++)
            {
                for(int j = searchTargetIndex; j < targetWord.Length; j++)
                {
                    if(inputWord[i] == targetWord[j])
                    {
                        searchTargetIndex = j + 1;
                        if (i != inputWord.Length - 1 && targetWord.Length <= searchTargetIndex) return false;
                        break;
                    }

                    if (j == targetWord.Length - 1) return false;
                }
            }

            return true;
        }
    }
}
