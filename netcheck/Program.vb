Imports System.IO
Imports System.Reflection

Public Module Program

    Private WithEvents CurrentAppDomain As AppDomain = AppDomain.CurrentDomain

    Public Function Main(ParamArray args As String()) As Integer

        Dim errors = 0

        For Each arg In args

            Try

                arg = Path.GetFullPath(arg)

                Dim asmname = AssemblyName.GetAssemblyName(arg)

                DisplayDependencies(New List(Of AssemblyName), Path.GetDirectoryName(arg), asmname, 0)

            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.Error.Write(ex.ToString())
                Console.Error.Write(": ")
                Console.Error.WriteLine(arg)
                Console.ResetColor()

                errors += 1

            End Try

        Next

        If Debugger.IsAttached Then
            Console.ReadKey()
        End If

        Return errors

    End Function

    Sub DisplayDependencies(asmlist As List(Of AssemblyName), basepath As String, asmname As AssemblyName, indentlevel As Integer)

        If asmlist.Find(Function(name) AssemblyName.ReferenceMatchesDefinition(name, asmname)) IsNot Nothing Then
            Return
        End If

        asmlist.Add(asmname)

        Dim asm As Assembly
        Try
            asm = Assembly.Load(asmname)

        Catch
            Try
                asmname = AssemblyName.GetAssemblyName(Path.Combine(basepath, $"{asmname.Name}.dll"))
                asm = Assembly.Load(asmname)

            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Error.WriteLine($"Error loading {asmname}: {ex.Message}")
                Console.ResetColor()
                Return

            End Try

        End Try

        Console.Write(New String(" "c, 2 * indentlevel))
        If asm.GlobalAssemblyCache Then
            Console.ForegroundColor = ConsoleColor.Green
        Else
            Console.ForegroundColor = ConsoleColor.White
            Console.Write($"{asm.Location}: ")
        End If
        Console.WriteLine(asmname.FullName)
        Console.ResetColor()

        For Each refasm In asm.GetReferencedAssemblies()

            DisplayDependencies(asmlist, basepath, refasm, indentlevel + 1)

        Next

    End Sub

    Private Sub CurrentAppDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs) Handles CurrentAppDomain.UnhandledException

        Console.ForegroundColor = ConsoleColor.Red
        Console.Error.WriteLine($"Exception: {e.ExceptionObject}")

    End Sub
End Module
