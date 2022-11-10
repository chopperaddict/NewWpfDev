using System;
using System . ComponentModel;
using System . ComponentModel . DataAnnotations;
using System . Diagnostics;


namespace NewWpfDev
{

    [Serializable]
    public class GenericClass
    {
        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged ( string PropertyName )
        {
            if ( this . PropertyChanged != null )
            {
                var e = new PropertyChangedEventArgs ( PropertyName );
                this . PropertyChanged ( this , e );
            }
        }

        public object Clone ( )
        {
            throw new NotImplementedException ( );
        }

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        //[Conditional("DEBUG")]
        //[DebuggerStepThrough]
        //public virtual void VerifyPropertyName(string propertyName)
        //{
        //    // Verify that the property name matches a real,
        //    // public, instance property on this object.
        //    if (TypeDescriptor.GetProperties(this)[propertyName] == null)
        //    {
        //        string msg = "Invalid property name: " + propertyName;

        //        if (this.ThrowOnInvalidPropertyName)
        //            throw new Exception(msg);
        //        else
        //            Debug.Fail(msg);
        //    }
        //}

        //public void BeginEdit() => throw new NotImplementedException();
        //public void CancelEdit() => throw new NotImplementedException();
        //public void EndEdit() => throw new NotImplementedException();

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        //protected virtual bool ThrowOnInvalidPropertyName
        //{
        //    get; private set;
        //}

        #endregion OnPropertyChanged

        #region declare variables for fields
        private string? _field1;
        public string field1
        {
            get => _field1!;
            set
            {
                _field1 = value; OnPropertyChanged ( nameof ( field1 ) );
            }
        }
        private string? _field2;
        public string field2
        {
            get => _field2!;
            set
            {
                _field2 = value; OnPropertyChanged ( nameof ( field2 ) );
            }
        }
        private string? _field3;
        public string field3
        {
            get => _field3!;
            set
            {
                _field3 = value; OnPropertyChanged ( nameof ( field3 ) );
            }
        }
        private string? _field4;
        public string field4
        {
            get => _field4!;
            set
            {
                _field4 = value; OnPropertyChanged ( nameof ( field4 ) );
            }
        }
        private string? _field5;
        public string field5
        {
            get => _field5!;
            set
            {
                _field5 = value; OnPropertyChanged ( nameof ( field5 ) );
            }
        }
        private string? _field6;
        public string field6
        {
            get => _field6!;
            set
            {
                _field6 = value; OnPropertyChanged ( nameof ( field6 ) );
            }
        }
        private string? _field7;
        public string field7
        {
            get => _field7!;
            set
            {
                _field7 = value; OnPropertyChanged ( nameof ( field7 ) );
            }
        }
        private string? _field8;
        public string field8
        {
            get => _field8!;
            set
            {
                _field8 = value; OnPropertyChanged ( nameof ( field8 ) );
            }
        }
        private string? _field9;
        public string field9
        {
            get => _field9!;
            set
            {
                _field9 = value; OnPropertyChanged ( nameof ( field9 ) );
            }
        }
        private string? _field10;
        public string field10
        {
            get => _field10!;
            set
            {
                _field10 = value; OnPropertyChanged ( nameof ( field10 ) );
            }
        }
        private string? _field11;
        public string field11
        {
            get => _field11!;
            set
            {
                _field11 = value; OnPropertyChanged ( nameof ( field11 ) );
            }
        }
        private string? _field12;
        public string field12
        {
            get => _field12!;
            set
            {
                _field12 = value; OnPropertyChanged ( nameof ( field12 ) );
            }
        }
        private string? _field13;
        public string field13
        {
            get => _field13;
            set
            {
                _field13 = value; OnPropertyChanged ( nameof ( field13 ) );
            }
        }
        private string? _field14;
        public string field14
        {
            get => _field14!;
            set
            {
                _field14 = value; OnPropertyChanged ( nameof ( field14 ) );
            }
        }
        private string? _field15;
        public string field15
        {
            get => _field15!;
            set
            {
                _field15 = value; OnPropertyChanged ( nameof ( field15 ) );
            }
        }
        private string? _field16;
        public string field16
        {
            get => _field16!;
            set
            {
                _field16 = value; OnPropertyChanged ( nameof ( field16 ) );
            }
        }
        private string? _field17;
        public string field17
        {
            get => _field17!;
            set
            {
                _field17 = value; OnPropertyChanged ( nameof ( field17 ) );
            }
        }
        private string? _field18;
        public string field18
        {
            get => _field18!;
            set
            {
                _field18 = value; OnPropertyChanged ( nameof ( field18 ) );
            }
        }
        private string? _field19;
        public string field19
        {
            get => _field19!;
            set
            {
                _field19 = value; OnPropertyChanged ( nameof ( field19 ) );
            }
        }
        private string? _field20;
        public string field20
        {
            get => _field20!;
            set
            {
                _field20 = value; OnPropertyChanged ( nameof ( field20 ) );
            }
        }
        #endregion declare variables for fields

    }
}