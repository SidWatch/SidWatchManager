using System;
using HDF5DotNet;

namespace SidWatchCollectionLibrary.Helpers
{
    public static class HD5Helper
    {
        public static void SetAttribute(H5ObjectWithAttributes _objectId, string _name, int _value)
        {

        }

        public static void SetAttribute(H5ObjectWithAttributes _objectId, string _name, double _value)
        {

        }

        public static void SetAttribute(H5ObjectWithAttributes _objectId, string _name, string _value)
        {
            long[] attributeDims = new long[1];
            var mtype = H5T.create(H5T.CreateClass.STRING, 255);

            H5DataSpaceId attributeSpace = H5S.create_simple(1, attributeDims);
            H5AttributeId attributeId = H5A.create(_objectId, _name, mtype, attributeSpace);

            var enc = System.Text.Encoding.ASCII;
            byte[] encoded = enc.GetBytes(_value);
            byte[] padded = new byte[255];

            int length = encoded.Length;
            if (length > 255)
            {
                length = 255;
            }

            Array.Copy(encoded, padded, length);
            H5Array<byte> data = new H5Array<byte>(padded);

            H5A.write(attributeId, mtype, data);

            H5T.close(mtype);
            H5S.close(attributeSpace);
            H5A.close(attributeId);
        }
    }
}
