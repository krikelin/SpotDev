/*
 * Created by SharpDevelop.
 * User: Alexander
 * Date: 2010-12-27
 * Time: 12:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
namespace Board
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public class MainForm
	{
		
			public static int Fade(float amount,int value)
			{
				float c = 0;
				if(amount<0){
					c=value+((value)*amount);
				}
				if(amount>0){
					c=value+((255-value)*amount);
				}
					
				return (int)Math.Round(c);
			}
			public static Color FadeColor(float amount,Color D)
			{
				int R = Fade(amount,D.R);
				int G = Fade(amount,D.G);
				int B = Fade(amount,D.B);
				return Color.FromArgb(R,G,B);
			}
		
		public MainForm()
		{
		}
	}
}
