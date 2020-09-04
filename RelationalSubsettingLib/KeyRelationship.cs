namespace RelationalSubsettingLib
{
    public class KeyRelationship
    {
        #region Public Fields

        public readonly Key Foreign;
        public readonly Key Primary;

        #endregion Public Fields

        #region Public Constructors

        public KeyRelationship(Key primary, Key foreign)
        {
            Primary = primary;
            Foreign = foreign;
        }

        #endregion Public Constructors

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(KeyRelationship))
            {
                return false;
            }
            else
            {
                KeyRelationship rel = (KeyRelationship)obj;
                return IsEqual(rel);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private bool IsEqual(KeyRelationship value)
        {
            bool equals = (
                Primary.FileName.Equals(value.Primary.FileName)
                && Primary.Column.Equals(value.Primary.Column)
                && Foreign.FileName.Equals(value.Foreign.FileName)
                && Foreign.Column.Equals(value.Foreign.Column)
                );
            return equals;
        }

        #endregion Private Methods
    }
}