
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace System.Collections
{
    /// <summary>
    /// BitArray addressable by enum values.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <remarks> Allowsmore flags per <typeparamref name="TEnum"/> than using c# [Flags] attribute.
    /// Removes the complexity of flags because each bit is a single enum flags. and bit management is eliminated.
    /// Implementation considers zero as an independant flag, not equivalent to no flags set. TODO: - should it?
    /// </remarks>
    public class EnumBitArray<TEnum>
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Maximum integer flags defined in <see cref="TEnum"/>
        /// </summary>
        /// <remarks> This is also the maximum number of bits required and the size of the internal <see cref=" BitArray"/></remarks>
        static readonly int _maxValue = Enum.GetValues<TEnum>().Max(v => ToFlagIndex(v)+1); // Zero based and index >= count.

        private BitArray _flags = new BitArray(_maxValue);
        /// <summary>
        /// Accesses the <see cref="BitArray"/> flags backing the enum Flags ]
        /// </summary>
        /// <flags>
        ///  <see cref="BitArray"/> of <see cref="TEnum"/>> mapped onto flags."/>
        /// </flags>
        public BitArray Flags
        {
            get => _flags;
            set { _flags = value; if (_flags.Length < _maxValue) _flags.Length = Length; }
        }


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBitArray{TEnum}"/> class.
        /// </summary>
        /// <remarks> Default constructor creates <see cref="EnumBitArray{TEnum}"/> sized for enum but with no bits set. </remarks>
        public EnumBitArray()
        { // Flags are set up using default values and property accessors.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBitArray{TEnum}"/> containing the <see cref=" BitArray"/>values passed in.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public EnumBitArray(BitArray flags)
        {
            Flags = new BitArray(flags); // deep copy.
        }

        public EnumBitArray(TEnum flag) : this() 
        { 
            Set(flag);
        }


        protected EnumBitArray(IEnumerable<TEnum> flags) : this()
        {
            Set(flags);
        }

        public static EnumBitArray<TEnum> Zero = new EnumBitArray<TEnum>(); // sized array with no values sets.

        /// <summary>
        /// Converts EnumValue  to index.
        /// </summary>
        /// <param name="enumValue">The enum flags.</param>
        /// <returns></returns>
        protected static int ToFlagIndex(TEnum enumValue) => Convert.ToInt32(enumValue);


        #endregion Constructors

        #region Operators


        // Equality operators

        public static bool operator ==(EnumBitArray<TEnum> bits, TEnum flag) => bits.Equals(flag);
        public static bool operator !=(EnumBitArray<TEnum> bits, TEnum flag) => !bits.Equals(flag);

        // Logicical operators.
        public static EnumBitArray<TEnum>  operator |(EnumBitArray<TEnum> left, EnumBitArray<TEnum> right)
        {
            left.Flags.Or(right.Flags);
            return left;
        }

        public static EnumBitArray<TEnum> operator |(EnumBitArray<TEnum> left, TEnum flag)
        {
            return left | new EnumBitArray<TEnum>(flag);
        }


        public static EnumBitArray<TEnum> operator ~(EnumBitArray<TEnum> operand)
        {
            var result = new EnumBitArray<TEnum>(operand.Flags);
            result.Flags.Not();
            return result;
        }


        public static EnumBitArray<TEnum> operator !(EnumBitArray<TEnum> left) => ~left;

        public static bool operator ==(EnumBitArray<TEnum> left, EnumBitArray<TEnum> right) => left.Equals(right);

        public static bool operator !=(EnumBitArray<TEnum> left, EnumBitArray<TEnum> right) => !left.Flags.Equals(right.Flags);

        // Type conversions

        /// <summary>
        /// Performs an implicit conversion from <see cref="TEnum"/> to <see cref="EnumBitArray{TEnum}"/>.
        /// </summary>
        /// <param name="enumValue">The enum flags.</param>
        /// <returns>
        /// <see cref="EnumBitArray{TEnum}"/> with the flag for <paramref name="enumValue"/> set."/>
        /// </returns>
        public static implicit operator EnumBitArray<TEnum>(TEnum enumValue) => new EnumBitArray<TEnum>(enumValue);

        // Don't impliment convertion to TEnum because you don't know which flag you want represented.

        /// <summary>
        /// Performs an implicit conversion from <see cref="EnumBitArray{TEnum}"/> to <see cref="BitArray"/>.
        /// </summary>
        /// <param name="value">The flags.</param>
        /// <returns>
        /// Returns the internal <see cref="BitArray"/> representing the flags of the flags.
        /// </returns>
        public static explicit operator BitArray(EnumBitArray<TEnum> value) => value.Flags;



        /// <summary>
        /// Performs an implicit conversion from <see cref="BitArray"/> to <see cref="EnumBitArray{TEnum}"/>.
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <returns>
        /// Creates a new <see cref="EnumBitArray{TEnum}"/> containing the source <see cref="BitArray"/>
        /// </returns>
        public static explicit operator EnumBitArray<TEnum>(BitArray flags) => new EnumBitArray<TEnum>(flags);

        
        #endregion Operators

        /// <summary>
        /// Gets the count of flags supported by the Enum.
        /// </summary>
        /// <flags>
        /// The count.
        /// </flags>
        public int Count => Flags.Count;
        public int Length { get => Flags.Length; set => Flags.Length = value; }

        /// <summary>
        /// Gets or sets the <see cref="System.Boolean"/> with the specified bit flag. for a given enum flags.
        /// </summary>
        /// <flags>
        /// The <see cref="System.Boolean"/>.
        /// </flags>
        /// <param name="flag">The flag.</param>
        /// <returns></returns>
        public bool this[TEnum flag] { get => Flags[ToFlagIndex(flag)]; set => Flags[ToFlagIndex(flag)] = value; }

        /// <summary>
        /// Ors current flags with the flag at position <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The flags.</param>
        public void Or(TEnum value) 
        { 
            Or (new EnumBitArray<TEnum>(value));
        }


        /// <summary>
        /// Ors the current flags with the flags in <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The flags.</param>
        /// <remarks seealso="bitArray.Or(BitArray)"/>
        public void Or(EnumBitArray<TEnum> value)
        {
            Flags.Or(value.Flags);
        }

        /// <summary>
        /// Ands the bits with the current bit values.
        /// </summary>
        /// <param name="value">The flags.</param>
        /// <seealso cref="BitArray.And(BitArray)"/>
        public void And(EnumBitArray<TEnum> value)
        {
            Flags.And(value.Flags);
        }

        /// <summary>
        /// Ands the flag flags at position <paramref name="value"/>. with the current flags.
        /// </summary>
        /// <param name="value">The flags.</param>
        public void And(TEnum value)
        {
            And(new EnumBitArray<TEnum>(value));
        }


        /// <summary>
        /// Inverts the bits in the current flags.
        /// </summary>
        /// <seealso cref="BitArray.Not"/>
        public void Not()
        {
            Flags.Not();
        }


        /// <summary>
        /// Sets the bit at position <paramref name="value"/> to true.
        /// </summary>
        /// <param name="value">The flag to set.</param>
        public void Set(TEnum value)
        {
            this[value] = true;
        }

        /// <summary>
        /// Sets the specified flags.
        /// </summary>
        /// <param name="flags"><see cref=" IEnumerable{T}" of flags to set. /></param>
        public void Set(IEnumerable<TEnum> flags)
        { foreach (TEnum flag in flags)
                Set(flag);
        }


        /// <summary>
        /// Set the bit at position <paramref name="flag"/> to false.
        /// </summary>
        /// <param name="flag">The flag.</param>
        public void UnSet(TEnum flag)
        {
            this[flag] = false;
        }

        /// <summary>
        /// Clears the flags in the flags enumeration.
        /// </summary>
        /// <param name="flags"><see cref=" IEnumerable{T}" of flags to clear /></param>
        public void UnSet(IEnumerable<TEnum> flags)
        {
            foreach (TEnum flag in flags)
                UnSet(flag);
        }

        /// <summary>
        /// Gets a flags indicating whether this instance has any flags set.
        /// </summary>
        /// <flags>
        ///   <c>true</c> if this instance hasflags  any set; otherwise, <c>false</c>.
        ///   <seealso cref="BitArray.HasAnySet"/>
        /// </flags>
        public bool HasAnySet() => Flags.HasAnySet();

        /// <summary>
        /// Are all the bit flags set?
        /// </summary>
        /// <flags>
        ///   <c>true</c> All the flags are set.<c>false</c>.
        /// </flags>
        public bool HasAllSet() => Flags.HasAllSet();

        /// <summary>
        /// Is the specified flag set?
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns>
        ///   <c>true</c> if the specified flag has flag; otherwise, <c>false</c>.
        /// </returns>
        public bool HasFlag(TEnum flag) => this[flag];
        /// <summary>
        /// Determines whether any flags other than the specified flag are set.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns>
        ///   <c>true</c> if [has other flags] [the specified flag]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOtherFlags(TEnum flag) => GetSetEnums().Any(f => (!f.Equals(flag)));

        /// <summary>
        /// Determines whether the specified flag is set.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <returns>
        ///   <c>true</c> if the specified flag is set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSet(TEnum flag) => HasFlag(flag);


        /// <summary>
        /// <seealso cref="Object.ToString()"/>
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> with a omma delimited list of the set enum values.representing set flags.
        /// </returns>
        public override string ToString()
            => string.Join(',',GetSetEnums());

        public override bool Equals(object? obj)
        {
            if (obj == null) return base.Equals(obj);

            // If this is an enum, try get the flags and check the flag.
            if (obj.GetType() == typeof(TEnum))
            {
                TEnum enumValue = (TEnum)obj;
                return IsSet(enumValue);
            }

            var equalTo = obj as EnumBitArray<TEnum>;
            if (equalTo is null)
                return base.Equals(obj);

            return Equals(equalTo);
        }

        public bool Equals(EnumBitArray<TEnum>? obj)
        {
            if (obj is null) return false;
            return Equals(Flags, obj.Flags);
        }


        public virtual bool Equals(TEnum enumFlag) => this[enumFlag];



        public IEnumerable<TEnum> GetSetEnums()
        {
            foreach (TEnum flag in Enum.GetValues<TEnum>())
                if (this[flag])
                    yield return flag;
            
        }

        protected bool Equals(BitArray compare, BitArray compareTo)
        { 
            if (compare.Length != compareTo.Length ) return false;
            return getFlags(compare).SequenceEqual(getFlags(compareTo));

            static IEnumerable<bool> getFlags(BitArray bits)
            { 
                foreach(var bit in bits) yield return (bool)bit;
            }
        }

        public IEnumerable<bool> GetFlags()
        {
            foreach (var flag in Flags)
                yield return (bool)flag;
            }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (TEnum e in GetSetEnums())
            {
                hash = hash ^= Convert.ToInt32(e);
            }
            //Console.WriteLine($"{ this.ToString() },  { hash }");
            return hash;
        }
        


    }
}