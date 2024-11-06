﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RestaurantManagement.Infrastructure.Converter;

public class UlidToStringConverter(ConverterMappingHints mappingHints = null) : ValueConverter<Ulid, string>(
        convertToProviderExpression: x => x.ToString(),
        convertFromProviderExpression: x => Ulid.Parse(x),
        mappingHints: defaultHints.With(mappingHints))
{
    private static readonly ConverterMappingHints defaultHints = new ConverterMappingHints(size: 26);

    public UlidToStringConverter() : this(null)
    {
    }
}
