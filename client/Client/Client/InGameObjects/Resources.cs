/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 26.04.2014
 * Time: 09:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Client
{
	/// <summary>
	/// Description of CardHand. 
	/// </summary>
	public class Resources
	{
		static private string[] Names = new string[4] {"Honor", "Schields", "Swords", "Supplies"};
		
		public int R1=0;
		public int R2=0;
		public int R3=0;
		public int R4=0;
		
		public int[] All
		{
			get
			{
				return new int[] {R1, R2, R3, R4};
			}
			set
			{
				if (value.Length == 4)
				{
					R1 = value[0];
					R2 = value[1];
					R3 = value[2];
					R4 = value[3];
				}
			}
		}
		
		public Resources(int r1, int r2, int r3, int r4)
		{
			R1 = r1;
			R2 = r2;
			R3 = r3;
			R4 = r4;
		}
		public Resources(int[] r)
		{
			if (r.Length == Names.Length)
			{
				R1 = r[0];
				R2 = r[1];
				R3 = r[2];
				R4 = r[3];
			}
		}
		
		public Resources() {}
		
		public override string ToString()
		{
			int[] Pos = new int[All.Length];
			int[] Neg = new int[All.Length];
			
			for (int i = 0; i < All.Length; i++)
			{
				if (All[i] > 0)
				{
					Pos[i] = All[i];
					Neg[i] = 0;
				}
				else
				{
					Pos[i] = 0;
					Neg[i] = -All[i];
				}
			}
			
			Resources Positive = new Resources(Pos);
			Resources Negative = new Resources(Neg);
			
			string Out = "";
			if (Positive != new Resources())
				Out += "gained " + Positive.StructureResources();
			if (Positive != new Resources() && Negative != new Resources())
				Out += " and ";
			if (Negative != new Resources())
				Out += "lost " + Negative.StructureResources();
			return Out;
		}
		
		private string StructureResources()
		{
			string Out = "";
			for (int i=0; i< All.Length; i++)
			{
				if (All[i] != 0)
					Out += All[i].ToString() + " " + Names[i] + ";";
			}
			return MainForm.StructureString(Out, new char[] {';'});
		}
		
		public static Resources operator + (Resources r1, Resources r2)
		{
			return new Resources(r1.R1+r2.R1,r1.R2+r2.R2,r1.R3+r2.R3,r1.R4+r2.R4);
		}
		public static Resources operator - (Resources r1, Resources r2)
		{
			return new Resources(r1.R1-r2.R1,r1.R2-r2.R2,r1.R3-r2.R3,r1.R4-r2.R4);
		}
		public static bool operator < (Resources r1, Resources r2)
		{
			return (r1.R1<r2.R1 && r1.R2<r2.R2 && r1.R3<r2.R3 && r1.R4<r2.R4);
		}
		public static bool operator > (Resources r1, Resources r2)
		{
			return (r1.R1>r2.R1 && r1.R2>r2.R2 && r1.R3>r2.R3 && r1.R4>r2.R4);
		}
		public static bool operator <= (Resources r1, Resources r2)
		{
			return (r1.R1<=r2.R1 && r1.R2<=r2.R2 && r1.R3<=r2.R3 && r1.R4<=r2.R4);
		}
		public static bool operator >= (Resources r1, Resources r2)
		{
			return (r1.R1>=r2.R1 && r1.R2>=r2.R2 && r1.R3>=r2.R3 && r1.R4>=r2.R4);
		}
		public static bool operator == (Resources r1, Resources r2)
		{
			return (r1.R1==r2.R1 && r1.R2==r2.R2 && r1.R3==r2.R3 && r1.R4==r2.R4);
		}
		public static bool operator != (Resources r1, Resources r2)
		{
			return (r1.R1!=r2.R1 || r1.R2!=r2.R2 || r1.R3!=r2.R3 || r1.R4!=r2.R4);
		}
	}
}
