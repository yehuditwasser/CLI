
using System.CommandLine;
using System.IO;

    //myfb bundle
    var bundleLang = new Option<List<string>>("--language", "list of languages");
    bundleLang.AddAlias("-l");
    bundleLang.IsRequired = true;

    var bundleOutput = new Option<string>("--output", "Name of the output file");
    bundleOutput.AddAlias("-o");
    bundleLang.IsRequired = true;

    var bundleNote = new Option<bool>("--note", () => false, "Write the source of the code");
    bundleNote.AddAlias("-n");

    var bundleSort = new Option<string>("--sort", () => "fn", "Sort the code files either by file name or by extension (parameters: fn or ex)");
    bundleSort.AddAlias("-s");

    var bundleRemove = new Option<bool>("--remove", () => false, "Remove empty lines before copying");
    bundleRemove.AddAlias("-r");

    var bundleAuthor = new Option<string>("--author", () => "", "Name of file author");
    bundleAuthor.AddAlias("-a");

    var bundleCmd = new Command("bundle", "file bundler - bundle cmd");

    bundleCmd.AddOption(bundleLang);
    bundleCmd.AddOption(bundleOutput);
    bundleCmd.AddOption(bundleNote);
    bundleCmd.AddOption(bundleSort);
    bundleCmd.AddOption(bundleRemove);
    bundleCmd.AddOption(bundleAuthor);

//foreach (var opt in bundleCmd.Options)
//{
//    opt.AddAlias("-" + opt.Name[2]);
//}

bundleCmd.SetHandler((languages, fileName, note, sort, remove, author) =>
    {
        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
        string res = "";

        if (sort == "ex")
        {               
            Array.Sort(files, (file1, file2) => file1.Split(".")[1].CompareTo(file2.Split(".")[1]));
        }
        else
        {
            Array.Sort(files);
        }

        if (author != "")
        {
            res += "Author of this file: " + author + "\n";
        }

        foreach (string file in files)
        {

            if (languages.Contains(file.Split(".")[1]) || languages.Contains("all"))
            {
                string[] lines = File.ReadAllLines(file);

                if (note)
                {
                    res += "\n" + file + "\n";
                }

                if (remove)
                {
                    var newLine = lines.Where(line => line != "");
                    res += string.Join("\n", newLine);
                }
                else
                {
                    res += string.Join("\n", lines) + "\n";
                }
            }
        }
        try
        {
            File.WriteAllText(fileName, res);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("file was created succefully in " + Directory.GetCurrentDirectory() + "\\" + fileName);
            Console.ResetColor();
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("File path is invalid");
            Console.ResetColor();
        }

    }, bundleLang, bundleOutput, bundleNote, bundleSort, bundleRemove, bundleAuthor);


    //myfb create-rsp
    var createRspCmd = new Command("create-rsp", "create response file");
    createRspCmd.SetHandler(() =>
    {

        Console.WriteLine("Enter file path");
        string path = Console.ReadLine();
        while (!Directory.Exists(path))
        {
            Console.WriteLine("Path is invalid, please enter a valid path");
            path = Console.ReadLine();
        }
        Console.WriteLine("Enter file name");
        string fileName = Console.ReadLine();
        string output = "-o " + '"' + path + "\\" + fileName + '"' + "\n";

        Console.WriteLine("Enter languages");
        string[] l = Console.ReadLine().Split(" ");
        string languages = "";
        foreach (string s in l)
        {
            languages += "-l " + s + " ";
        }
        languages += "\n";
       
        Console.WriteLine("Do you want to know the code source? (Y/N)");
        string answer = Console.ReadLine().ToLower();
        while(answer != "y" && answer != "n")
        {
            Console.WriteLine("Please enter Y/N");
            answer = Console.ReadLine();
        }
        string note = answer=="y" ? note="-n true\n" : note = "-n false\n";

        Console.WriteLine("How do you want to sort the files? (fn/ex)");
        answer = Console.ReadLine();
        while( answer != "fn" && answer != "ex")
        {
            Console.WriteLine("Please enter fn/ex");
            answer = Console.ReadLine();
        }
        string sort = "-s " + answer + "\n";

        Console.WriteLine("Do you want to remove empty lines? (Y/N)");
        answer = Console.ReadLine();
        while (answer != "y" && answer != "n")
        {
            Console.WriteLine("Please enter Y/N");
            answer = Console.ReadLine();
        }
        string remove = answer.ToLower() == "y" ? remove = "-r true\n" : note = "-r false\n";

        Console.WriteLine("Enter your name if you want...");
        string author = "-a " + '"' + Console.ReadLine() + '"' + "\n";

        try
        {
            File.WriteAllText("response.rsp", output + languages + note + sort + remove + author);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("response file was created succefully in " + Directory.GetCurrentDirectory() + "\\response.rsp");
            Console.ResetColor();
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Directory path is invalid");
            Console.ResetColor();
        }
    });



    var rootCmd = new RootCommand("file bundler - root command");
    rootCmd.AddCommand(bundleCmd);
    rootCmd.AddCommand(createRspCmd);

    rootCmd.InvokeAsync(args);
