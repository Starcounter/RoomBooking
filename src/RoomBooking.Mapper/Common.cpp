static option<entity> lookup_system(const string &sys_name)
{
    return from(M::Entity::by_String(sys_name)).filter(Name).object_of(HaveRelation).having_subject(System).first();
}

const featureset get_relation(bool is_tracked)
{
    return is_tracked ? UserRelation.Untracked() : UserRelation;
}

static M::Entity create_user(const featureset &subject, const string &sys_name)
{
    auto system = lookup_system(sys_name);
    if (!system)
    {
        system = entity(System.create());
        HaveRelation.ensure_related(*system, Name.ensure_string(sys_name));
    }

    return UserRelation.create_with().subject(subject.create()).object(system).go();
}

static bool check_root_has_correct_system(const M::Entity &root, const string &sys_name)
{
    return from(root).having_object(System).subject_of(HaveRelation).having_object(Name).filter(sys_name).first().has_value();
}

static void inval_user_relation(const featureset &F, const entity &obj, entity::invset &inval, const entity::changes &changes, const featureset &subject, bool is_tracked = false)
{
    const auto relation = get_relation(is_tracked);

    if (relation.test(F))
    {
        inval(obj);
    }

    if (subject.test(F))
    {
        from(obj).subject_of(relation).for_each(inval);
    }

    if (System.test(F))
    {
        from(obj).object_of(relation).for_each(inval);
    }

    if (HaveRelation.test(F))
    {
        from(obj.subject()).filter(relation).for_each(inval);
        from(changes.subject_assigned() ? changes.subject() : none).filter(relation).for_each(inval);
        from(obj.subject()).filter(System).object_of(relation).for_each(inval);
        from(changes.subject_assigned() ? changes.subject() : none).filter(System).object_of(relation).for_each(inval);
    }

    if (Name.test(F))
    {
        from(obj).object_of(HaveRelation).having_subject(System).object_of(relation).for_each(inval);
    }
}

static bool refresh_user_relation(const entity &root, const featureset &subject, const string &sys_name, bool is_tracked = false)
{
    if (!get_relation(is_tracked).test(root))
    {
        return false;
    }

    const auto agent = from(root).having_subject(subject).first();
    if (!agent)
    {
        return false;
    }

    if (!check_root_has_correct_system(root, sys_name))
    {
        return false;
    }

    return true;
}
