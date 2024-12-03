/*
This snippet allows to set an operation in the Sequence Editor by specifying its name and class.
Additionally, it allows to get the duration of the operation.
*/
using System;
using System.IO;
using System.Windows.Forms;
using Tecnomatix.Engineering;
using System.Linq;
using Tecnomatix.Engineering.Olp;
public class MainScript
{
    public static void MainWithOutput(ref StringWriter output)
    {
        /*
        There are basically 2 types of operations we are really interested in:
        1) Robot operations ==> TxContinuousRoboticOperation
        2) Human operations ==> TxHumanTsbSimulationOperation
        */
        // Set (in the sequence editor) the desired operation by specifying its name
        var descendants = TxApplication.ActiveDocument.OperationRoot.GetAllDescendants(new TxTypeFilter(typeof(TxContinuousRoboticOperation)));
        TxContinuousRoboticOperation op = null;
        foreach (var descendant in descendants)
        {
            if (descendant.Name.Equals("RoboticProgram_0"))
            {
                op = descendant as TxContinuousRoboticOperation;
                break; // Exit loop after finding the first match
            }
        }
        TxApplication.ActiveDocument.CurrentOperation = op;
        TxSimulationPlayer player = TxApplication.ActiveDocument.SimulationPlayer;
        player.Rewind();
        player.Play();
        // Get the duration of the operation
        string Time = op.Duration.ToString();
        output.Write(Time);
    }
}