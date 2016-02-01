# Mata
### Mapping + Data
Easily map data from DbDataReader to Poco objects, or from Poco to a set of DbCommand parameters.  

Mata is fast!  This is as fast as hand tuned ADO.Net code after the map is created the first time.

Mata also has a great debugging experience so that you dont get that "WTF" moment you get with other ORMs.

#### Why stay close to ADO.Net?
You can use DbDataReader/DbCommand that you already know, and still have an easy to maintain transformation to/from POCO objects.

In addition, by building on top of ADO.Net instead of introducing a new query or database syntax, you can use async, you can use other tools that work with ADO.Net (like Transient Application Block).  

If there is something you need to do that is not supported by Mata, you can still fall back to normal ADO.Net.

## Installation

Mata is on nuget:

[https://www.nuget.org/packages/Mata](https://www.nuget.org/packages/Mata)

    Install-Package Mata -Pre 

## Basic Usage

Full documentation is at [our wiki](https://github.com/cleverguy25/Mata/wiki)

#### Setup map definition

    public class FooModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    var mapDefinition = new MapDefinition<FooModel>();
    mapDefinition.MapType() // Accept default mappings (meaning DB column names )
    // See full documentation for more examples on explicit mapping

#### Basic DbDataReader to POCO


Then once you have executed your reader and have your reader:
    
    var map = CreateMap(reader, mapDefinition);

    var list = new List<T>();
    while (await reader.ReadAsync())
    {
        var item = LoadItem(reader, map);
        list.Add(item);
    }

    return list;   

Or just use our helper method:

    return await reader.LoadListAsync(mapDefinition);

#### Poco to DbCommand Parameters

First initialize your command setting command text, command type, whatever you want.  Then to map parameters:
    // model is of type FooModel
    command.LoadParameters(model);

#### Debugging

Having a type conversion or other issue and want to see what Mata is doing?

Just turn on debug symbols!

    MapEmitAssembly.EmitDebugSymbols = true;

Then step into a LoadOrdinals, Load, or LoadParameters call.  VS will ask you where the cs file, it will be in your bin directory.  You can then step through the code that Mata is generating.

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D