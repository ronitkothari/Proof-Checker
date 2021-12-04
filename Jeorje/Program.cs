﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Jeorje
{
    class Program
    {
        static void Main(string[] args)
        {



            var jeorjeInput = @"
#check ND

a <=> b,
c <=> b
|-
a <=> c

1) a <=> b premise
2) c <=> b premise
3) a <=> c by trans on 1, 2
                "; 
                    
                    
                    // "this is just a test\n" +
                    //           "#check ND\n" +
                    //           "!a\n" +
                    //           "|-\n" +
                    //           "z | !z\n" +
                    //           "\n" +
                    //           "0) !a premise\n" +
                    //           "1) !z | z by lem\n" 
                    //           ;
            
            string output;
            
            try
            {
                var tokens = Scanner.MaximalMunchScan(jeorjeInput);
                Logger.AddStep("Successfully scanned input tokens");

                var proofFormat = Transformer.TransformTokens(tokens);
                Logger.AddStep($"Found proof format, performing {proofFormat.CheckType}");

                switch (proofFormat.CheckType)
                {
                    case CheckType.ND:
                        var ndFormat = proofFormat as NDFormat;
                        List<AST> ndPremises = Parser.ParseLines(ndFormat.Premises);
                        AST ndGoal = Parser.ParseLine(ndFormat.Goal);
                        Logger.AddStep("Parsed premises and goal");
                        
                        List<NDRule> ndProof = NDRulifier.RulifyLines(ndFormat.Proof);
                        Logger.AddStep("Rulify ND worked");
                        
                        Logger.AddStep(Validator.ValidateND(ndPremises, ndGoal, ndProof));
                        break;

                    default:
                        throw new Exception($"Check type {proofFormat.CheckType.ToString()} not supported yet");
                }
            }
            
            catch (Exception e)
            {
                 Logger.AddError($"Exception thrown:\n{e.Message}");
            }
            
            Console.WriteLine(Logger.LogAll());
            
        }
    }
}
