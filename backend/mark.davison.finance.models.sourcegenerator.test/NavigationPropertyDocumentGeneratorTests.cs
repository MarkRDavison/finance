using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mark.davison.finance.models.sourcegenerator.test;
[TestClass]
public class NavigationPropertyDocumentGeneratorTests
{


    [TestMethod]
    public void TestMethod1()
    {
        const string namespaceName = "mark.davison.finance.models.Entities";
        var generator = new NavigationPropertyDocumentGenerator(namespaceName);
        var content = generator.GenerateNavigationProperties("Account");
    }
}