using System;
using System.ComponentModel;
using System.Globalization;

namespace GameStudio.Repository
{
    public abstract class Mapper<T1, T2> : TypeConverter
        where T1: new()
        where T2: new()
    {
        public abstract T2 Map(T1 source, T2 destination);
        public abstract T1 Map(T2 source, T1 destination);

        public virtual T2 Map(T1 source)
        {
            return Map(source, new T2());
        }

        public virtual T1 Map(T2 source)
        {
            return Map(source, new T1());
        }

        bool CanConvert(Type type)
        {
            return typeof(T1) == type ||
                   typeof(T2) == type;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return CanConvert(sourceType) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return CanConvert(destinationType) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (source is T1 one)
                return Map(one, new T2());

            if (source is T2 two)
                return Map(two, new T1());

            return base.ConvertFrom(context, culture, source);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object source, Type destinationType)
        {
            if (source is T1 one && destinationType == typeof(T2))
                return Map(one, new T2());

            if (source is T2 two && destinationType == typeof(T1))
                return Map(two, new T1());

            return base.ConvertTo(context, culture, source, destinationType);
        }
    }
}
