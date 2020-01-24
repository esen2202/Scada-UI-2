using System;
using System.ComponentModel;

namespace Scada.model.DBs
{
    public class User : INotifyPropertyChanged
    {
        public long User_ID { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public bool SuperUser { get; set; }
        public bool Enable { get; set; }
        public int Authorization { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        public string Title { get; set; }
        public string Position { get; set; }
        public string PhotoAddress { get; set; }
        public string CardID { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            var propertyChangedEventHandler = this.PropertyChanged;

            if (propertyChangedEventHandler != null)
            {
                propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
