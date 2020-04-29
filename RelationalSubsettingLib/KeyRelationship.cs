namespace RelationalSubsettingLib
{
    public class KeyRelationship
    {
        public readonly Key Primary;
        public readonly Key Foreign;

        public KeyRelationship(Key primary, Key foreign)
        {
            Primary = primary;
            Foreign = foreign;
        }
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
    }
}