using FilterExpression.Directive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterExpression.DirectiveDispatcher
{
    internal class DirectiveDispatchService
    {
        private readonly Dictionary<string, IFilterDirective> _directiveDictionary = new Dictionary<string, IFilterDirective>();

        public DirectiveDispatchService()
        {
            var directiveType = typeof(IFilterDirective);

            var result = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => directiveType.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x) as IFilterDirective)
                .GroupBy(x => x.FilterSyntax)
                .Select(g => g.First())
                .ToDictionary(x => x.FilterSyntax.ToLower(), x => x);

            if (result != null)
                _directiveDictionary = result;
        }

        public IFilterDirective GetDirective(string directiveName)
        {
            var found = _directiveDictionary.TryGetValue(directiveName.ToLower(), out var directive);

            if (!found)
                throw new ArgumentException("Directive name is not valid");

            return directive;
        }
    }
}
