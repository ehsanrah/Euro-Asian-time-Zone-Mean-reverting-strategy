//MNQ 15
#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies.EhsanStrats
{
	public class MidNightDream : Strategy
	{
		bool CanIGoLong	 	 = false;
		bool CanIGoShort	 = false;
		bool upwardMom    	 = false;
		bool downwardMom    	 = false;
		bool highStrength    = false;
		bool highVolatility  = false;
		bool highVol		 = false;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"MidNightDream";
				Name										= "MidNightDream";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 2;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;IncludeCommission = true;
				IncludeCommission = true;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				StarttimeMidNightDream = DateTime.Parse("19:00", System.Globalization.CultureInfo.InvariantCulture);
           		EndtimeMidNightDream = DateTime.Parse("09:29", System.Globalization.CultureInfo.InvariantCulture);
				qtyMidNightDreamL											= 1;
				qtyMidNightDreamS											= 2;
				
			// Strategy Settings
                LongPTMidNightDream										= 250;
                LongSLMidNightDream	 									= 100;
                ShortPTMidNightDream	 									= 200;
                ShortSLMidNightDream										= 150;
				SignalNameMidNightDreamL 									= "MidNightDream";
    			SignalNameMidNightDreamS 									= "MidNightDream";
				numBar1													= 2;

			// Filter Settings
				ADXVal													= 20;
				ADXLen													= 10;
				RSILen													= 15;
				RSIValShort												= 45;
				RSIValLong												= 55;
				VOLSmallLen												= 13;
				VOLLargeLen												= 15;
				ATRSmallLen												= 1;
				ATRLargeLen												= 10;
				SMALen													= 30;

				

			}
			else if (State == State.Configure)
			{
				SetProfitTarget(CalculationMode.Ticks, LongPTMidNightDream);
				SetStopLoss(CalculationMode.Ticks, LongSLMidNightDream);
				//SetProfitTarget(CalculationMode.Ticks, ShortPTMidNightDream);
				//SetStopLoss(CalculationMode.Ticks, ShortSLMidNightDream);
			}
		}
		protected override void OnBarUpdate()
	    {
	        if (CurrentBars[0] < 20) return; // Wait until enough bars for SMA calculation
			DateTime currentTime = Time[0];
			//Initialization
			CanIGoLong	 	 = false;
			CanIGoShort	     = false;
			upwardMom    	 = false;
			downwardMom      = false;
			highStrength     = false;
			highVolatility   = false;
			highVol		     = false;
			// upward Direction
			if ( 
				Close[0] > SMA(SMALen)[0] &&
				RSI(RSILen,3)[0] < RSIValLong
				)
			{
				upwardMom = true;
			}
			// downward Direction
			if ( 
				Close[0] < SMA(SMALen)[0] &&
				RSI(RSILen,3)[0] > RSIValShort
				)
			{
				downwardMom = true;
			}
			// Strength
			if  ( 
				ADX(ADXLen)[0] > ADXVal
				)
			{
				highStrength = true;
			}
			// Volatility
			if  ( 
				ATR(ATRSmallLen)[0] > ATR(ATRLargeLen)[0]
				)
			{
				highVolatility = true;
			}
			// Volume
			if  ( 
				SMA(VOL(),VOLSmallLen)[0] > SMA(VOL(),VOLLargeLen)[0]
				)
			{
				highVol = true;
			}
			
			// Can I go Long
			
			if  ( 
				upwardMom && highStrength && highVolatility && !highVol
				)
			{
				CanIGoLong = true;
			}
			// Can I go Short
			
			if  ( 
				downwardMom && highStrength && highVolatility && !highVol
				)
			{
				CanIGoShort = true;
			}
			
			if (currentTime.TimeOfDay >= EndtimeMidNightDream.TimeOfDay && Position.MarketPosition == MarketPosition.Long) ExitLong();
			if (currentTime.TimeOfDay >= EndtimeMidNightDream.TimeOfDay && Position.MarketPosition == MarketPosition.Short) ExitShort();			
									
			if (EndtimeMidNightDream.TimeOfDay < StarttimeMidNightDream.TimeOfDay)
        	{
	            if (currentTime.TimeOfDay >= StarttimeMidNightDream.TimeOfDay || currentTime.TimeOfDay < EndtimeMidNightDream.TimeOfDay)
	            {
					if ((High[2]<High[3] && Low[2]>Low[3]) ||
   						(High[1]>High[2] && Low[1]<Low[2]) ||
   						(High[0]<High[1] && Low[0]>Low[1])) 
					{
						if (Close[0]>Close[numBar1] && CanIGoLong)
							{
		                    	EnterLong(qtyMidNightDreamL, SignalNameMidNightDreamL);
		                	}

						if (Close[0]<Close[numBar1] && CanIGoShort)
							{
		                    	EnterShort(qtyMidNightDreamS, SignalNameMidNightDreamS);
			                }
					}
	            }
	        }
	        else
	        {
	            if (currentTime.TimeOfDay >= StarttimeMidNightDream.TimeOfDay && currentTime.TimeOfDay < EndtimeMidNightDream.TimeOfDay)
	            {
					if (currentTime.TimeOfDay >= StarttimeMidNightDream.TimeOfDay || currentTime.TimeOfDay < EndtimeMidNightDream.TimeOfDay)
		            {
						if ((High[2]<High[3] && Low[2]>Low[3]) ||
   						(High[1]>High[2] && Low[1]<Low[2]) ||
   						(High[0]<High[1] && Low[0]>Low[1])) 
						{
							if (Close[0]>Close[numBar1] && CanIGoLong)
								{
			                    	EnterLong(qtyMidNightDreamL, SignalNameMidNightDreamL);
			                	}

							if (Close[0]<Close[numBar1] && CanIGoShort)
								{
			                    	EnterShort(qtyMidNightDreamS, SignalNameMidNightDreamS);
				                }
						}
		            }
	            }
	        }
		}

		
		#region Properties

		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Threshold",  Order = 0,  GroupName = "Filter settings")]
		public double ADXVal
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Length",  Order = 0,  GroupName = "Filter settings")]
		public int ADXLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RSI Threshold Short",  Order = 0,  GroupName = "Filter settings")]
		public double RSIValShort
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RSI Threshold Long",  Order = 0,  GroupName = "Filter settings")]
		public double RSIValLong
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RSI Length",  Order = 0,  GroupName = "Filter settings")]
		public int RSILen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SMA Length",  Order = 0,  GroupName = "Filter settings")]
		public int SMALen
		{get; set;}
		
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VOL Length Small",  Order = 0,  GroupName = "Filter settings")]
		public int VOLSmallLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VOL Length Large",  Order = 0,  GroupName = "Filter settings")]
		public int VOLLargeLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Length Small",  Order = 0,  GroupName = "Filter settings")]
		public int ATRSmallLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Length Large",  Order = 0,  GroupName = "Filter settings")]
		public int ATRLargeLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Look back number 1",  Order = 0,  GroupName = "Strategy settings")]
		public int numBar1
		{get; set;}
		
		
		[NinjaScriptProperty]
		[Display(Name = "Signal Name MidNightDream Long", Description = "Signal name for MidNightDream Long entry", Order = 400, GroupName = "xName Settings")]
		public string SignalNameMidNightDreamL { get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Signal Name MidNightDream Short", Description = "Signal name for MidNightDream Short entry", Order = 400, GroupName = "xName Settings")]
		public string SignalNameMidNightDreamS { get; set; }
		
		[NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Profit Tick Target", Order = 1,  GroupName = "MidNightDream")]
        public double LongPTMidNightDream { get; set; }

        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Stop Tick Loss", Order = 2,  GroupName = "MidNightDream")]
        public double LongSLMidNightDream { get; set; }
		
        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Profit Tick Target", Order = 3,  GroupName = "MidNightDream")]
        public double ShortPTMidNightDream { get; set; }

        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Stop Tick Loss", Order = 4,  GroupName = "MidNightDream")]
        public double ShortSLMidNightDream { get; set; }
	 	
        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
        [Display(Name = "Session Start 1", Description = "Trading Session Start Time", Order = 7,  GroupName = "MidNightDream")]
        public DateTime StarttimeMidNightDream
        { get; set; }
 		
        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
        [Display(Name = "Session End 1", Description = "Trading Session End Time", Order = 8,  GroupName = "MidNightDream")]
        public DateTime EndtimeMidNightDream
    	{ get; set; }
	   
        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Contract Quantity", Order = 7, GroupName = "MidNightDream")]
        public int qtyMidNightDreamL
        { get; set; }

        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Contract Quantity", Order = 7, GroupName = "MidNightDream")]
        public int qtyMidNightDreamS
        { get; set; }
		#endregion
	}
}
