using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mark.davison.finance.models.sourcegenerator
{

    public class NavigationPropertyDocumentGenerator
    {
        private string _namespace;
        public NavigationPropertyDocumentGenerator(string _namespace)
        {
            this._namespace = _namespace;
        }
        public string GenerateNavigationProperties(ITypeSymbol entity, List<string> entities)
        {

            var members = entity.GetMembers().Where(x => x.DeclaredAccessibility == Accessibility.Public && x.Kind == SymbolKind.Property);

            string content =
$@"using System;

#nullable enable

namespace mark.davison.finance.models.Entities {{

    public partial class {entity.Name} {{

";
            foreach (var member in members)
            {
                if (member.Name.EndsWith("Id"))
                {
                    var totalEntityMatch = member.Name.Substring(0, member.Name.Length - 2);
                    var perfectMatch = entities.FirstOrDefault(_ => string.Equals(_, totalEntityMatch));
                    var imperfectMatch = entities.FirstOrDefault(_ => totalEntityMatch.EndsWith(_));

                    if (perfectMatch != null)
                    {
                        content += $@"        public virtual {perfectMatch}? {totalEntityMatch} {{ get; set; }}" + Environment.NewLine + Environment.NewLine;
                    }
                    else if (imperfectMatch != null)
                    {
                        content += $@"        public virtual {imperfectMatch}? {totalEntityMatch} {{ get; set; }}" + Environment.NewLine + Environment.NewLine;
                    }
                }
            }

            content += $@"
    }}
	
}}
";


            return content;
        }
    }

}