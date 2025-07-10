using Microsoft.VisualStudio.TestTools.UnitTesting;

// need for correct discovery of some of the test cases which uses DynamicData
// https://github.com/microsoft/testfx/issues/905#issuecomment-907775327
[assembly: TestDataSourceDiscovery(TestDataSourceDiscoveryOption.DuringExecution)]
