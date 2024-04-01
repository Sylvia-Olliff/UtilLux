using Nuke.Common.Tooling;
using Nuke.Common;

namespace UtilLux.Build;

public partial class Build
{
    [PathVariable("codesign")]
    private readonly Tool? codeSign;

    [PathVariable("pkgbuild")]
    private readonly Tool? pkgBuild;

    [PathVariable("productbuild")]
    private readonly Tool? productBuild;

    [PathVariable("xcrun")]
    private readonly Tool? xCodeRun;

    [PathVariable("security")]
    private readonly Tool? security;

    private Tool CodeSign =>
        this.codeSign.NotNull("codesign is not available")!;

    private Tool PkgBuild =>
        this.pkgBuild.NotNull("pkgbuild is not available")!;

    private Tool ProductBuild =>
        this.productBuild.NotNull("productbuild is not available")!;

    private Tool XCodeRun =>
        this.xCodeRun.NotNull("xcrun is not available")!;

    private Tool Security =>
        this.security.NotNull("security is not available")!;
}
