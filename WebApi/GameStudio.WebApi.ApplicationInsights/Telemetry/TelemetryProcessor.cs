using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace GameStudio.WebApi.ApplicationInsights.Telemetry
{
	public class TelemetryProcessor : ITelemetryProcessor
	{
		private readonly ITelemetryProcessor _next;
		private static ITelemetryContext _ctx;

		public ITelemetryContext _context
		{
			get { return _ctx; }
			set { _ctx = value; }
		}

		public string AppName { get; set; }

		public TelemetryProcessor(ITelemetryProcessor next)
		{
			_next = next;
		}

		public void Process(ITelemetry item)
		{
			bool stop = false;
			if (!(null == _context))
			{
				if (!(null == _context?.TelemetryConditions))
				{
					var applicableCondition
					= _context.TelemetryConditions.Where(x
					=> x.Type.ToString() == item.GetType().Name).FirstOrDefault();
					if (!(applicableCondition == null))
					{
						if (item is OperationTelemetry)
							stop = CheckOperationTelemetry(applicableCondition.Condition, item as OperationTelemetry);
						else if (item is TraceTelemetry)
							stop = CheckTraceTelemetry(applicableCondition.Condition, item as TraceTelemetry);
						else if (item is RequestTelemetry)
							stop = CheckRequestTelemetry(applicableCondition.Condition, item as RequestTelemetry);
					}
				}
			}
			if (!(stop || null == _next))
				_next.Process(item);
		}

		private bool CheckRequestTelemetry(Dictionary<LeftOperand, IEnumerable<string>> Condition, RequestTelemetry telemetry)
		{
			bool stop = false;
			short index = 0;
			while ((!stop) && (index < Condition.Count))
			{
				var condititon = Condition.ElementAtOrDefault(index++);
				if (!condititon.Equals(null))
				{
					switch (condititon.Key)
					{
						case LeftOperand.OperationName:
							var opName = telemetry?.Context?.Operation?.Name;
							if (!string.IsNullOrWhiteSpace(opName))
								stop = condititon.Value.Any(x => x.ToLower().Contains(opName.ToLower()));
							break;
						default:
							break;
					}
				}
			}
			return stop;
		}

		private bool CheckOperationTelemetry(Dictionary<LeftOperand, IEnumerable<string>> Condition, OperationTelemetry telemetry)
		{
			bool stop = false;
			short index = 0;
			while ((!stop) && (index < Condition.Count))
			{
				var condititon = Condition.ElementAtOrDefault(index++);
				if (!condititon.Equals(null))
				{
					switch (condititon.Key)
					{
						case LeftOperand.OperationName:
							var opName = telemetry?.Context?.Operation?.Name;
							if (!string.IsNullOrWhiteSpace(opName))
								stop = condititon.Value.Any(opName.ToLower().Contains)
										   ||
									  condititon.Value.Any(x => x.ToLower().Contains(opName.ToLower()));
							break;
						default:
							break;
					}
				}
			}
			return stop;
		}


		private bool CheckTraceTelemetry(Dictionary<LeftOperand, IEnumerable<string>> Condition, TraceTelemetry telemetry)
		{
			bool stop = false;
			short index = 0;
			while ((!stop) && (index < Condition.Count))
			{
				var condititon = Condition.ElementAtOrDefault(index++);
				if (!condititon.Equals(null))
				{
					switch (condititon.Key)
					{
						case LeftOperand.EndpointName:
							if (telemetry.Properties.Any(i => i.Key.Contains(LeftOperand.EndpointName.ToString())))
								stop = condititon.Value.Any(x
									=> x.ToLower().Contains(telemetry.Properties[LeftOperand.EndpointName.ToString()].ToLower()));
							break;
						case LeftOperand.OperationName:
							var opName = telemetry?.Context?.Operation?.Name;
							if (!string.IsNullOrWhiteSpace(opName))
								stop = condititon.Value.Any(opName.ToLower().Contains)
											||
									   condititon.Value.Any(x => x.ToLower().Contains(opName.ToLower()));
							break;
						case LeftOperand.Path:
							if (telemetry.Properties.ContainsKey(LeftOperand.Path.ToString()))
								stop = condititon.Value.Any(x
									=> x.ToLower().Contains(telemetry.Properties[LeftOperand.Path.ToString()].ToLower()));
							break;
						default:
							break;
					}
				}
			}
			return stop;
		}
	}

}
