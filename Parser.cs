
using System;



public class Parser {
	public const int _EOF = 0;
	public const int _number = 1;
	public const int _ident = 2;
	public const int maxT = 12;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

public double calculateExpression(double leftOp, double rightOp, string operation)
{
	Console.WriteLine(operation);
	switch (operation) {
		case "PLUS":
			return leftOp + rightOp;
		case "MINUS":
			return leftOp - rightOp;
		case "MUL":
			return leftOp * rightOp;
		case "DIV":
			return leftOp / rightOp;
		case "POW":
			return Math.Pow(leftOp, rightOp);
		default:
			Console.WriteLine("Invalid operation symbol: {operation}", operation);
			return 0;
	}
}



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Sample() {
		double n; 
		while (StartOf(1)) {
			Expr(out n);
			Console.WriteLine(n); 
		}
	}

	void Expr(out double n ) {
		double n1; 
		HigherExpr(out n);
		string op = "undefined"; 
		while (la.kind == 3 || la.kind == 4) {
			if (la.kind == 3) {
				Get();
				op = "PLUS"; 
			} else {
				Get();
				op = "MINUS"; 
			}
			HigherExpr(out n1);
			n = calculateExpression(n, n1, op); 
		}
	}

	void HigherExpr(out double n ) {
		double n1; 
		singleExpr(out n);
		string op = "undefined"; 
		while (la.kind == 5 || la.kind == 6 || la.kind == 7) {
			if (la.kind == 5) {
				Get();
				op = "MUL"; 
			} else if (la.kind == 6) {
				Get();
				op = "DIV"; 
			} else {
				Get();
				op = "POW"; 
			}
			singleExpr(out n1);
			n = calculateExpression(n, n1, op); 
		}
	}

	void singleExpr(out double n ) {
		n = 0.0; 
		if (la.kind == 1 || la.kind == 2) {
			Term(out n);
		} else if (la.kind == 8) {
			Get();
			Term(out n);
			Expect(9);
			n = Math.Sin(n); 
		} else if (la.kind == 10) {
			Get();
			Term(out n);
			Expect(9);
			n = Math.Exp(n); 
		} else if (la.kind == 11) {
			Get();
			Term(out n);
			Expect(9);
			n = Math.Cos(n); 
		} else SynErr(13);
	}

	void Term(out double n) {
		n = 0; 
		if (la.kind == 1) {
			Get();
			n = Convert.ToDouble(t.val); 
		} else if (la.kind == 2) {
			Get();
			Console.WriteLine("Invalid operation symbol: " + t.val); 
		} else SynErr(14);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Sample();

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x},
		{_x,_T,_T,_x, _x,_x,_x,_x, _T,_x,_T,_T, _x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "number expected"; break;
			case 2: s = "ident expected"; break;
			case 3: s = "\"+\" expected"; break;
			case 4: s = "\"-\" expected"; break;
			case 5: s = "\"*\" expected"; break;
			case 6: s = "\"/\" expected"; break;
			case 7: s = "\"^\" expected"; break;
			case 8: s = "\"sin(\" expected"; break;
			case 9: s = "\")\" expected"; break;
			case 10: s = "\"exp(\" expected"; break;
			case 11: s = "\"cos(\" expected"; break;
			case 12: s = "??? expected"; break;
			case 13: s = "invalid singleExpr"; break;
			case 14: s = "invalid Term"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
