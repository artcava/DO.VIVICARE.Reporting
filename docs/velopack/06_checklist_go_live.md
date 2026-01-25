# ✅ CHECKLIST PRE-RILASCIO
## Velopack Implementation Validation

**Data:** 25 Gennaio 2026  
**Status:** Pre-Launch

---

## FASE 1: DEVELOPMENT

- [ ] Velopack NuGet package aggiunto
- [ ] Program.cs modificato
- [ ] ConfigurationService con %AppData%
- [ ] UpdateService implementato
- [ ] MainForm integrata con updates
- [ ] appsettings.json configurato
- [ ] No hardcoded paths rimasti
- [ ] Build locale: `dotnet build` ✅
- [ ] Tests passano: `dotnet test` ✅
- [ ] No compiler warnings
- [ ] Config persiste tra runs
- [ ] Update check completes without hang
- [ ] PR reviewed by team lead ✅
- [ ] No merge conflicts
- [ ] Ready to merge master

---

## FASE 2: CI/CD SETUP

- [ ] `.github/workflows/ci-cd.yml` aggiornato
- [ ] Build job completes successfully
- [ ] Test job completes successfully
- [ ] Release job configured for tags
- [ ] `vpk pack` command validated
- [ ] MSI generation confirmed
- [ ] `CODESIGN_CERTIFICATE_BASE64` added
- [ ] `CODESIGN_PASSWORD` added
- [ ] Secrets visible in Actions settings
- [ ] Create test tag: `git tag v0.1.0-test`
- [ ] Push tag: `git push origin v0.1.0-test`
- [ ] Watch workflow run ✅
- [ ] Workflow completes successfully
- [ ] Artifacts generated (nupkg, setup.exe, msi)
- [ ] Delete test tag

---

## FASE 3: SECURITY & SIGNING

- [ ] Certificate imported locally
- [ ] Certificate valid (not expired)
- [ ] Certificate has Code Signing EKU
- [ ] Certificate password secure
- [ ] Backup certificate stored securely
- [ ] GitHub secret encrypted correctly
- [ ] Verify signed MSI: `signtool verify /pa app.msi`
- [ ] No signature warnings
- [ ] No secrets in code
- [ ] No hardcoded credentials
- [ ] HTTPS used for update checks

---

## FASE 4: LOCAL TESTING

- [ ] Download MSI locally
- [ ] Double-click → installer starts
- [ ] Accept license → proceed
- [ ] Installation completes successfully
- [ ] App in Add/Remove Programs
- [ ] Start menu shortcut created
- [ ] App launches without errors
- [ ] UI renders correctly
- [ ] All main features work
- [ ] Config loads from %AppData%
- [ ] Data persists after close/reopen
- [ ] Update check completes
- [ ] Uninstall works correctly
- [ ] Config remains after uninstall
- [ ] Multi-user machine test
- [ ] Admin vs User install: both work

---

## FASE 5: DOCUMENTATION

- [ ] `04_installation_guide_users.md` complete
- [ ] `05_admin_guide_enterprise.md` complete
- [ ] `02_guida_implementazione.md` up-to-date
- [ ] All code examples tested
- [ ] Troubleshooting guide complete
- [ ] `RELEASE_NOTES.md` created for v1.0.0
- [ ] New features listed
- [ ] Known issues noted

---

## FASE 6: STAKEHOLDER SIGN-OFF

- [ ] Feature scope approved
- [ ] Release date confirmed
- [ ] User impact assessed
- [ ] No blocking issues
- [ ] Deployment method approved
- [ ] Security review passed
- [ ] Network requirements met
- [ ] Support readiness confirmed

---

## FASE 7: PRE-LAUNCH

- [ ] All code committed to master
- [ ] No uncommitted changes
- [ ] Dry-run workflow passes locally
- [ ] No build artifacts committed
- [ ] Event logging configured
- [ ] Support team trained
- [ ] Release announcement drafted
- [ ] User notification scheduled
- [ ] IT team briefed

---

## FASE 8: GO-LIVE EXECUTION

### Final Checks
- [ ] All team members online
- [ ] Communication channels open
- [ ] Monitoring dashboard live
- [ ] Support team on standby
- [ ] Rollback plan reviewed

### Release Steps
1. [ ] Create tag: `git tag v1.0.0 -m "Release v1.0.0"`
2. [ ] Push tag: `git push origin v1.0.0`
3. [ ] Monitor workflow in GitHub Actions
4. [ ] Verify artifacts in Releases
5. [ ] Download and test MSI final time
6. [ ] Send release announcement
7. [ ] Monitor support channels (2 hours)
8. [ ] Document any issues
9. [ ] Retrospective scheduled

### Post-Launch Monitoring (48 hours)
- [ ] Zero critical issues
- [ ] Installation success rate > 99%
- [ ] Update checks working
- [ ] Support tickets < baseline

---

## ROLLBACK PLAN

If v1.0.0 has critical issue:

1. [ ] Delete release from GitHub
2. [ ] Issue statement: "Do not install v1.0.0"
3. [ ] Fix issue in codebase
4. [ ] Test thoroughly
5. [ ] Create v1.0.1 patch
6. [ ] Push: `git tag v1.0.1`

---

## SIGN-OFF

| Role | Name | Date | Sign-Off |
|------|------|------|----------|
| Developer | | | ☐ |
| QA | | | ☐ |
| DevOps | | | ☐ |
| Product Owner | | | ☐ |
| IT Manager | | | ☐ |

---

**Status:** 🔴 NOT READY  
**When all checked:** 🟢 READY TO LAUNCH
