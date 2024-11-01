namespace RestaurantManagement.Application.Extentions;

public static class PrimaryKeyGenerator
{
    public static int GeneratePrimaryKey()
    {
        string quantity = ReadDataFromFile(GetTempFilePath());
        int quantityInt = int.Parse(quantity);
        int newQuantity = quantityInt + 1;

        //ghi đè
        WriteDataToFile(GetTempFilePath(), newQuantity.ToString());

        return quantityInt;
    }

    public static string ReadDataFromFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public static string GetTempFilePath()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.Parent.Parent.FullName;
        return Path.Combine(projectDirectory,"RestaurantManagement.Application", "Extentions", "temp.txt");
    }

    public static void WriteDataToFile(string filePath, string data)
    {
        File.WriteAllText(filePath, data);
    }
}



