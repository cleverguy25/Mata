// <copyright file="ShortGuid.cs" company="Palador Open Source">
// Copyright (c) Palador Open Source. All rights reserved. See License.txt in the project root for license information.
// </copyright>

namespace Mata.Test
{
    using System;

    public struct ShortGuid
    {
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        private readonly Guid guid;

        private readonly string value;

        public ShortGuid(string value)
        {
            this.value = value;
            this.guid = Decode(value);
        }

        public ShortGuid(Guid guid)
        {
            this.value = Encode(guid);
            this.guid = guid;
        }

        public Guid Guid
        {
            get { return this.guid; }
        }

        public string Value
        {
            get { return this.value; }
        }

        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid.value;
        }

        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid.guid;
        }

        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }

        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        public static ShortGuid NewGuid()
        {
            return new ShortGuid(Guid.NewGuid());
        }

        public static string Encode(string value)
        {
            var guid = new Guid(value);
            return Encode(guid);
        }

        public static string Encode(Guid guid)
        {
            var encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
                .Replace("/", "_")
                .Replace("+", "-");
            return encoded.Substring(0, 22);
        }

        public static Guid Decode(string value)
        {
            value = value
                .Replace("_", "/")
                .Replace("-", "+");
            var buffer = Convert.FromBase64String(value + "==");
            return new Guid(buffer);
        }

        public override string ToString()
        {
            return this.value;
        }

        public override bool Equals(object compare)
        {
            if (compare is ShortGuid)
            {
                return this.guid.Equals(((ShortGuid)compare).guid);
            }

            if (compare is Guid)
            {
                return this.guid.Equals((Guid)compare);
            }

            var stringValue = compare as string;
            if (stringValue != null)
            {
                return this.guid.Equals(((ShortGuid)stringValue).guid);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.guid.GetHashCode();
        }
    }
}